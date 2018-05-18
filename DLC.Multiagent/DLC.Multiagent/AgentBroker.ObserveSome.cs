using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public IObservable<T> ObserveSome<TAgent, T>(IEnumerable<string> agentIds, string propertyName, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (agentIds == null) throw new ArgumentNullException("agentIds");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			return Observable.Merge(agentIds.Select(agentId => ObserveOne<T>(agentId, propertyName, ignoreAgentState)));
		}
	}
}