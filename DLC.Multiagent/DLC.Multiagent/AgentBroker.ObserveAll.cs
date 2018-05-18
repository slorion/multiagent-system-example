using System;
using System.Linq;
using System.Reactive.Linq;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public IObservable<Tuple<AgentInformation, IObservable<T>>> ObserveAll<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return ObserveAllUnsafe<TAgent, T>(propertyName, scope, ignoreAgentState).Select(t => Tuple.Create(t.Item1, t.Item2.SelectLeft(left => left)));
		}

		private IObservable<Tuple<AgentInformation, IObservable<Either<T, Exception>>>> ObserveAllUnsafe<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			var currentAgents = GetAgentInfos<TAgent>(scope);

			if (!ignoreAgentState)
				currentAgents = currentAgents.Where(a => a.IsReachable && a.LastKnownState == AgentState.Activated);

			var newAgentsSource = this.AgentDataSource
				.Where(a =>
					(scope.HasFlag(a.IsLocal ? ExecutionScopeOptions.Local : ExecutionScopeOptions.Remote))
					&& a.Contracts.Contains(typeof(TAgent).AssemblyQualifiedName));

			if (!ignoreAgentState)
				newAgentsSource = newAgentsSource.Where(a => a.IsReachable && a.LastKnownState == AgentState.Activated);

			return Observable
				.Merge(currentAgents.ToObservable(), newAgentsSource)
				.Distinct(a => a.AgentId)
				.Select(a => Tuple.Create(a, ObserveOneUnsafe<T>(a.AgentId, propertyName, ignoreAgentState)));
		}
	}
}