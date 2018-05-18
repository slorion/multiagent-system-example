using DLC.Multiagent.Logging;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Agents;
using DLC.Scientific.Core.Configuration;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	public abstract class ProviderAgent<TProvider, TData, TAgentConfiguration, TModuleConfiguration>
		: OperationalAgent<TAgentConfiguration, TModuleConfiguration>, IProviderAgent<TData>
		where TProvider : AcquisitionProvider<TData>
		where TData : ProviderData
		where TAgentConfiguration : AcquisitionAgentConfiguration
		where TModuleConfiguration : AcquisitionModuleConfiguration
	{
		private readonly Lazy<TProvider> _provider;
		private readonly List<Tuple<IDisposable, AcquisitionStep>> _observers = new List<Tuple<IDisposable, AcquisitionStep>>();

		public ProviderAgent()
			: base()
		{
			_provider = new Lazy<TProvider>(CreateAndConfigureProvider, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public TProvider Provider { get { return _provider.Value; } }
		public ProviderState ProviderState { get { return this.Provider.State; } }
		public IObservable<ProviderState> ProviderStateDataSource { get { return this.Provider.ProviderStateDataSource; } }
		public IObservable<TData> DataSource { get { return this.Provider.DataSource; } }

		protected virtual Task OnBeforeStateTransition(AcquisitionStep step, AcquisitionParameter parameters, AcquisitionActionResult result) { Log.Debug().Message("Before transition to AcquisitionStep '{0}'.", step).WithAgent(this.Id).Write(); return Task.FromResult(0); }
		protected virtual Task OnAfterStateTransition(AcquisitionStep step, AcquisitionParameter parameters, AcquisitionActionResult result) { Log.Debug().Message("After transition to AcquisitionStep '{0}'.", step).WithAgent(this.Id).Write(); return Task.FromResult(0); }

		protected async Task<AcquisitionActionResult> MakeStateTransition<TParameter>(AcquisitionStep step, Func<Func<Task<bool>>, Func<Task>, Task> moduleAction, TParameter parameters, Func<TParameter, AcquisitionActionResult, Task<AcquisitionActionResult>> agentAction)
			where TParameter : AcquisitionParameter
		{
			if (moduleAction == null) throw new ArgumentNullException("moduleAction");
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (agentAction == null) throw new ArgumentNullException("agentAction");

			var result = new AcquisitionActionResult(parameters.SequenceId, step, this.Id, this.DisplayData.Name) { MachineName = Environment.MachineName };

			// action to accomplish the state transition on the agent
			// if it succeeds, the module action will then be executed
			Func<Task<bool>> before =
				async () =>
				{
					try
					{
						await OnBeforeStateTransition(step, parameters, result).ConfigureAwait(false);

						result = await agentAction(parameters, result).ConfigureAwait(false);

						// Il faut désabonner les observers avant de faire la transition d'état,
						// car le code qui réagit aux événements assume que le provider n'est pas dans l'état de fin d'observation.
						// Les observers doivent également devenir inactifs, même si la transition d'état a planté.
						for (int i = _observers.Count - 1; i >= 0; i--)
						{
							if (_observers[i].Item2 == step)
							{
								_observers[i].Item1.Dispose();
								_observers.RemoveAt(i);
							}
						}

						return result.IsSuccessful;
					}
					catch
					{
						this.OperationalState |= OperationalAgentStates.InternalAgentError;
						throw;
					}
				};

			try
			{
				if (this.OperationalState != OperationalAgentStates.None)
					throw new InvalidOperationException(string.Format("{0} -> agent is not operational.", this.DisplayData.Name));

				await moduleAction(before, null).ConfigureAwait(false);
			}
			catch (InvalidStateTransitionException ex)
			{
				Log.Error().Exception(ex).WithAgent(this.Id).Write();
				result.Exception = ex;
				result.IsSuccessful = false;
			}
			catch (Exception ex)
			{
				if (this.ProviderState == ProviderState.Failed)
					this.OperationalState |= OperationalAgentStates.ModuleError;

				result.Exception = ex;
				result.IsSuccessful = false;
			}

			result.ProviderState = this.ProviderState;
			await OnAfterStateTransition(step, parameters, result).ConfigureAwait(false);

			if (result.IsSuccessful)
				Log.Debug().Message("Transition to AcquisitionStep '{0}' successful.", step).WithAgent(this.Id).Write();
			else
			{
				var logBuilder = Log.Warn().Message("Transition to AcquisitionStep '{0}' failed.", step);

				if (result.Exception != null)
					logBuilder = logBuilder.Exception(result.Exception);

				logBuilder.WithAgent(this.Id).Write();
			}

			return result;
		}

		/// <summary>
		/// Track an observer so that it may be unsubscribed at the specified acquisition step.
		/// </summary>
		/// <param name="observer">The observer to track.</param>
		/// <param name="unregisterStep">The acquisition step when the observer should unsubscribed.</param>
		protected void RegisterObserver(IDisposable observer, AcquisitionStep unregisterStep)
		{
			if (observer == null) throw new ArgumentNullException("observer");

			_observers.Add(Tuple.Create(observer, unregisterStep));
		}

		protected override RootConfiguration<TAgentConfiguration, TModuleConfiguration> LoadConfiguration()
		{
			return AcquisitionConfigurationFactory.Instance.LoadFromFile<TAgentConfiguration, TModuleConfiguration>(this.ConfigurationFilePath);
		}

		protected virtual TProvider CreateAndConfigureProvider()
		{
			return (TProvider) this.Configuration.Module.Provider;
		}

		protected override async Task<bool> ActivateCore()
		{
			if (!await base.ActivateCore().ConfigureAwait(false))
				return false;

			RegisterObserver(this.ProviderStateDataSource
				.Subscribe(
					state =>
					{
						if (state == ProviderState.Failed)
							this.OperationalState |= OperationalAgentStates.ModuleError;
						else
							this.OperationalState &= ~OperationalAgentStates.ModuleError;
					},
					ex => this.OperationalState |= OperationalAgentStates.ModuleError));

			var result = await this.Initialize(new InitializeAcquisitionParameter()).ConfigureAwait(false);

			if (result.IsSuccessful)
				result = await this.Start(new StartAcquisitionParameter()).ConfigureAwait(false);

			return result.IsSuccessful;
		}

		protected override async Task<bool> DeactivateCore()
		{
			AcquisitionActionResult result = null;
			if (this.ProviderState >= ProviderState.Starting)
				result = await this.Stop(new StopAcquisitionParameter()).ConfigureAwait(false);

			if ((result == null || result.IsSuccessful) && (this.ProviderState >= ProviderState.Initializing))
				result = await this.Uninitialize(new UninitializeAcquisitionParameter()).ConfigureAwait(false);

			// stop all observers no matter the current AcquisitionStep since the agent is becoming inactive
			foreach (var observer in _observers)
				observer.Item1.Dispose();
			_observers.Clear();

			return await base.DeactivateCore().ConfigureAwait(false) && (result == null || result.IsSuccessful);
		}

		protected override void DisposeCore(bool disposing)
		{
			if (_observers != null)
			{
				foreach (var observer in _observers)
					observer.Item1.Dispose();
				_observers.Clear();
			}

			if (_provider != null && _provider.IsValueCreated && _provider.Value != null)
				_provider.Value.Dispose();

			base.DisposeCore(disposing);
		}

		#region IProviderAgent members

		public TData CurrentData { get { return this.Provider != null ? this.Provider.CurrentData : default(TData); } }
		public long DataReceivedCount { get { return this.Provider != null ? this.Provider.DataReceivedCount : 0; } }

		public Task<AcquisitionActionResult> Initialize(InitializeAcquisitionParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.Initialize, this.Provider.Initialize, parameters, InitializeCore);
		}
		protected virtual Task<AcquisitionActionResult> InitializeCore(InitializeAcquisitionParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		public Task<AcquisitionActionResult> Start(StartAcquisitionParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.Start, this.Provider.Start, parameters, StartCore);
		}
		protected virtual Task<AcquisitionActionResult> StartCore(StartAcquisitionParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		public Task<AcquisitionActionResult> Stop(StopAcquisitionParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.Stop, this.Provider.Stop, parameters, StopCore);
		}
		protected virtual Task<AcquisitionActionResult> StopCore(StopAcquisitionParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		public Task<AcquisitionActionResult> Uninitialize(UninitializeAcquisitionParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.Uninitialize, this.Provider.Uninitialize, parameters, UninitializeCore);
		}
		protected virtual Task<AcquisitionActionResult> UninitializeCore(UninitializeAcquisitionParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		#endregion
	}
}