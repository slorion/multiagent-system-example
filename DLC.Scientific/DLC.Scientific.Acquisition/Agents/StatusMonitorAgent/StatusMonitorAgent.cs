using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.StatusMonitorAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Agents;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.StatusMonitorAgent
{
	public class StatusMonitorAgent
		: OperationalAgent<AcquisitionAgentConfiguration, AcquisitionModuleConfiguration>, IStatusMonitorAgent, IVisibleAgent
	{
		private DeferredSubject<Tuple<SerializableAgentInformation, ProviderState>> _agentsProviderStateSubject = new SubjectSlim<Tuple<SerializableAgentInformation, ProviderState>>().ToDeferred();

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(StatusMonitorUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IStatusMonitorAgent).AssemblyQualifiedName;
		}

		protected override async Task<bool> ActivateCore()
		{
			if (!await base.ActivateCore().ConfigureAwait(false))
				return false;

			this.RegisterObserver(AgentBroker.Instance.ObserveAll<IOperationalAgent, OperationalAgentStates>("OperationalStateDataSource", ignoreAgentState: true)
				.Subscribe(obs => this.RegisterObserver(obs.Item2.Subscribe(state => _agentsProviderStateSubject.OnNext(Tuple.Create(new SerializableAgentInformation(obs.Item1), ToProviderState(state)))))));

			this.RegisterObserver(AgentBroker.Instance.ObserveAll<IProviderAgent, ProviderState>("ProviderStateDataSource", ignoreAgentState: true)
				.Subscribe(obs => this.RegisterObserver(obs.Item2.Subscribe(state => _agentsProviderStateSubject.OnNext(Tuple.Create(new SerializableAgentInformation(obs.Item1), state))))));

			return true;
		}

		public IObservable<Tuple<SerializableAgentInformation, ProviderState>> AgentsProviderStateDataSource { get { return _agentsProviderStateSubject; } }

		public Task QueryStateForAllAgents()
		{
			return Task.WhenAll(
				AgentBroker.Instance.TryExecuteOnAll<IOperationalAgent, ProviderState>(a => ToProviderState(a.OperationalState), ignoreAgentState: true)
					.Concat(AgentBroker.Instance.TryExecuteOnAll<IProviderAgent, ProviderState>(agent => agent.ProviderState, ignoreAgentState: true))
					.Select(
						t1 => t1.ContinueWith(
							t2 =>
							{
								var result = t2.Result;
								AgentInformation agentInfo;
								if (AgentBroker.Instance.TryGetAgentInformation(result.AgentId, out agentInfo))
								{
									agentInfo = new SerializableAgentInformation(agentInfo);
									if (!result.IsSuccessful)
										_agentsProviderStateSubject.OnNext(Tuple.Create((SerializableAgentInformation) agentInfo, ProviderState.Failed));
									else
										_agentsProviderStateSubject.OnNext(Tuple.Create((SerializableAgentInformation) agentInfo, result.Result));
								}
							}, TaskContinuationOptions.OnlyOnRanToCompletion)
				));
		}

		private static ProviderState ToProviderState(OperationalAgentStates state)
		{
			if (state == OperationalAgentStates.None)
				return ProviderState.Started;
			else if (state == OperationalAgentStates.AgentNotRunning)
				return ProviderState.Created;
			else
				return ProviderState.Failed;
		}

		protected override void DisposeCore(bool disposing)
		{
			base.DisposeCore(disposing);

			// disposal of the DeferredSubject must be done after the base dispose has completed
			// to avoid the case where observers add elements after the subject has been disposed
			if (_agentsProviderStateSubject != null)
				_agentsProviderStateSubject.Dispose();
		}
	}
}