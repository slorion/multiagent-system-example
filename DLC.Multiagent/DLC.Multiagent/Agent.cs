using DLC.Framework.Reactive;
using DLC.Multiagent.Configuration;
using DLC.Multiagent.Logging;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	public abstract class Agent
		: IAgent
	{
		private readonly BehaviorSubjectSlim<AgentState> _stateSubject = new BehaviorSubjectSlim<AgentState>(AgentState.Created);

		void IAgent.LoadConfiguration(string agentId, AgentConfiguration configuration)
		{
			if (configuration == null) throw new ArgumentNullException("configuration");
			if (this.State != AgentState.Created) throw new InvalidOperationException(string.Format("Configuration has already been loaded for agent '{0}'.", this.Id));

			this.Id = agentId;
			this.DisplayData = new AgentDisplayData(configuration.Name, configuration.ShortName, configuration.Description);
			this.ConfigurationFilePath = configuration.ConfigurationFilePath;
			var result = MakeStateTransition(new[] { AgentState.Created }, AgentState.Created, AgentState.Idle, null).Result;
		}

		public AgentState State { get { return _stateSubject.Value; } }
		public IObservable<AgentState> StateDataSource { get { return _stateSubject.DistinctUntilChanged().ObserveOn(NewThreadScheduler.Default); } }

		public string Id { get; private set; }
		public AgentDisplayData DisplayData { get; private set; }
		public string ConfigurationFilePath { get; private set; }

		private async Task<bool> MakeStateTransition(IEnumerable<AgentState> validStates, AgentState intermediateState, AgentState toState, Func<Task<bool>> action)
		{
			if (validStates == null) throw new ArgumentNullException("validStates");

			var fromState = this.State;

			if (this.State == AgentState.Failed)
			{
				Log.Warn().Message("AgentState transition ('{0}->{1}') tried on an agent in a failed state.", intermediateState, toState).WithAgent(this).Write();
				return false;
			}

			if (!validStates.Contains(fromState))
				throw new InvalidOperationException(string.Format("Current state '{0}' is invalid, it must be one of the following: '{1}'.", fromState, string.Join("','", validStates)));

			Log.Debug().Message("Transition AgentState intermédiaire: {0}->{1}.", fromState, intermediateState).WithAgent(this).Write();
			_stateSubject.OnNext(intermediateState);

			bool result = false;

			try
			{
				result = (action == null) ? true : await action().ConfigureAwait(false);
			}
			finally
			{
				Log.Level(result ? LogLevel.Debug : LogLevel.Warn).Message("AgentState transition {3}: {0}->{1}->{2}.", fromState, intermediateState, toState, result ? "was successful" : "failed").WithAgent(this).Write();

				if (result)
					_stateSubject.OnNext(toState);
				else
					_stateSubject.OnNext(AgentState.Failed);
			}

			return result;
		}

		public Task<bool> Activate()
		{
			if (this.State == AgentState.Activating || this.State == AgentState.Activated)
				return Task.FromResult(true);

			return MakeStateTransition(new[] { AgentState.Idle }, AgentState.Activating, AgentState.Activated, ActivateCore);
		}
		protected virtual Task<bool> ActivateCore() { return Task.FromResult(true); }

		public Task<bool> Deactivate()
		{
			if (this.State == AgentState.Created || this.State == AgentState.Idle || this.State == AgentState.Deactivating || this.State == AgentState.Disposed)
				return Task.FromResult(true);

			return MakeStateTransition(new[] { AgentState.Activating, AgentState.Activated }, AgentState.Deactivating, AgentState.Idle, DeactivateCore);
		}
		protected virtual Task<bool> DeactivateCore() { return Task.FromResult(true); }

		public bool Ping()
		{
			return true;
		}

		public override string ToString()
		{
			return this.Id;
		}

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.State == AgentState.Disposed)
				return;

			this.DisposeCore(disposing);

			if (disposing)
			{
#pragma warning disable 4014
				MakeStateTransition((AgentState[]) Enum.GetValues(typeof(AgentState)), this.State, AgentState.Disposed, null);
#pragma warning restore 4014
			}
		}

		protected virtual void DisposeCore(bool disposing) { }

		~Agent()
		{
			Log.Warn().Message("Object was not disposed correctly.").WithAgent(this.Id).Write();
			Dispose(false);
		}

		#endregion
	}
}