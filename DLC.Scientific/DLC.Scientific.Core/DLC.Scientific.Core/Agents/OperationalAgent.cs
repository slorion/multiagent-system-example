using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Multiagent.Logging;
using DLC.Scientific.Core.Configuration;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Scientific.Core.Agents
{
	/// <summary>
	/// Represents an agent that manages an operational state.
	/// </summary>
	public abstract partial class OperationalAgent<TAgentConfiguration, TModuleConfiguration>
		: Agent, IOperationalAgent
		where TAgentConfiguration : AgentConfiguration
		where TModuleConfiguration : ModuleConfiguration
	{
		private readonly Lazy<string> _systemVersion = new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version.ToString(), LazyThreadSafetyMode.PublicationOnly);
		private readonly Lazy<RootConfiguration<TAgentConfiguration, TModuleConfiguration>> _configuration;

		private readonly List<IDisposable> _observers = new List<IDisposable>();
		private readonly BehaviorSubjectSlim<OperationalAgentStates> _operationalStateSubject = new BehaviorSubjectSlim<OperationalAgentStates>(OperationalAgentStates.AgentNotRunning);

		private readonly Dictionary<Type, AgentDependency> _dependencies = new Dictionary<Type, AgentDependency>();
		private readonly SubjectSlim<Tuple<string, string, OperationalAgentStates>> _dependenciesOperationalStateSubject = new SubjectSlim<Tuple<string, string, OperationalAgentStates>>();

		public string SystemVersion { get { return _systemVersion.Value; } }
		protected RootConfiguration<TAgentConfiguration, TModuleConfiguration> Configuration { get { return _configuration.Value; } }

		public OperationalAgent()
		{
			_configuration = new Lazy<RootConfiguration<TAgentConfiguration, TModuleConfiguration>>(LoadConfiguration, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		protected override async Task<bool> ActivateCore()
		{
			if (!await base.ActivateCore().ConfigureAwait(false))
				return false;

			this.OperationalState &= ~OperationalAgentStates.AgentNotRunning;

			ConfigureAgent();
			SetupAgentOperationalCommunications();

			return true;
		}

		protected virtual RootConfiguration<TAgentConfiguration, TModuleConfiguration> LoadConfiguration() { return new ConfigurationFactory().LoadFromFile<TAgentConfiguration, TModuleConfiguration>(this.ConfigurationFilePath); }
		protected virtual void ConfigureAgent() { }

		protected override Task<bool> DeactivateCore()
		{
			this.OperationalState |= OperationalAgentStates.AgentNotRunning;
			_dependencies.Clear();

			foreach (var observer in _observers)
				observer.Dispose();
			_observers.Clear();

			return base.DeactivateCore();
		}

		protected void TrackDependencyOperationalState<TAgent>(bool isMandatory = true, bool canFailover = true)
			where TAgent : IOperationalAgent
		{
			lock (_dependencies)
			{
				var dependency = new AgentDependency(isMandatory);
				_dependencies[typeof(TAgent)] = dependency;

				IObservable<Tuple<AgentInformation, OperationalAgentStates>> obs;
				if (canFailover)
					obs = AgentBroker.Instance.ObserveAnyWithAgentInfo<TAgent, OperationalAgentStates>("OperationalStateDataSource");
				else
					obs = AgentBroker.Instance.ObserveFirstWithAgentInfo<TAgent, OperationalAgentStates>("OperationalStateDataSource");

				this.RegisterObserver(obs.Subscribe(
					t =>
					{
						dependency.State = t.Item2;
						_dependenciesOperationalStateSubject.OnNext(Tuple.Create(typeof(TAgent).AssemblyQualifiedName, t.Item1.AgentId, t.Item2));
						ValidateAgentDependencies();
					}));
			}
		}

		private bool IsDependencyOperational(Type agentType)
		{
			if (!agentType.IsInterface || !typeof(IOperationalAgent).IsAssignableFrom(agentType)) throw new ArgumentException("agentType must be the type of an interface that extends IOperationalAgent.", "agentType");

			AgentDependency dependency;
			if (!_dependencies.TryGetValue(agentType, out dependency))
				throw new InvalidOperationException(string.Format("The dependency on agents of type '{0}' is not tracked.", agentType));
			else
				return dependency.State == OperationalAgentStates.None;
		}

		public bool IsDependencyOperational<T>()
			where T : IOperationalAgent
		{
			return IsDependencyOperational(typeof(T));
		}

		private void ValidateAgentDependencies()
		{
			lock (_dependencies)
			{
				var missing = _dependencies.Where(d => d.Value.IsMandatory && d.Value.State != OperationalAgentStates.None);

				if (missing.Any())
				{
					this.OperationalState |= OperationalAgentStates.ExternalMandatoryAgentMissing;
					Log.Warn().Message("Missing required dependencies: '{0}'", string.Join(",", missing.Select(d => d.Key))).WithAgent(this.Id).Write();
				}
				else
				{
					this.OperationalState &= ~OperationalAgentStates.ExternalMandatoryAgentMissing;
				}
			}
		}

		private void SetupAgentOperationalCommunications()
		{
			SetupAgentOperationalCommunicationsCore();
			ValidateAgentDependencies();
		}
		protected virtual void SetupAgentOperationalCommunicationsCore() { }

		/// <summary>
		/// Track an observer to be able to unsubscribe when deactivating (<see cref="Deactivate"/>).
		/// </summary>
		/// <param name="observer">The observer to track.</param>
		protected void RegisterObserver(IDisposable observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");
			_observers.Add(observer);
		}

		protected override void DisposeCore(bool disposing)
		{
			this.OperationalState |= OperationalAgentStates.AgentNotRunning;

			if (_observers != null)
			{
				foreach (var observer in _observers)
					observer.Dispose();
				_observers.Clear();
			}

			base.DisposeCore(disposing);
		}

		#region IOperationalAgent members

		public IObservable<Tuple<string, string, OperationalAgentStates>> DependenciesOperationalStateDataSource { get { return _dependenciesOperationalStateSubject; } }

		public IObservable<OperationalAgentStates> OperationalStateDataSource { get { return _operationalStateSubject.DistinctUntilChanged(); } }

		public OperationalAgentStates OperationalState
		{
			get { return _operationalStateSubject.Value; }
			set
			{
				if (value != OperationalAgentStates.None && value != OperationalAgentStates.AgentNotRunning)
					Log.Warn().Message("Failed operational state: '{0}'.", value).WithAgent(this.Id).Write();

				_operationalStateSubject.OnNext(value);
			}
		}

		public bool IsDependencyOperational(string agentTypeName)
		{
			if (string.IsNullOrWhiteSpace(agentTypeName)) throw new ArgumentNullException("agentTypeName");
			return IsDependencyOperational(Type.GetType(agentTypeName, true, true));
		}

		#endregion

		#region IVisibleAgent stubs

		public bool AutoShowUI { get; set; }
		public bool ShowErrorListOnLoad { get; set; }
		public string MainUITypeName { get; set; }
		public string MainUIAgentTypeName { get; set; }

		#endregion
	}
}