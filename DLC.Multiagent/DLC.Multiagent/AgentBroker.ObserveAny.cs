using System;
using System.Reactive.Linq;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public IObservable<T> ObserveAny<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			return ObserveFirstUnsafe<TAgent, T>(propertyName, scope, ignoreAgentState)
				.Select(
					r =>
					{
						if (!r.IsLeft)
							throw r.Right;
						else
							return r.Left;
					})
				.Retry();
		}

		public IObservable<Tuple<AgentInformation, T>> ObserveAnyWithAgentInfo<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			return ObserveFirstUnsafeWithAgentInfo<TAgent, T>(propertyName, scope, ignoreAgentState)
				.Select(
					t =>
					{
						if (!t.Item2.IsLeft)
							throw t.Item2.Right;
						else
							return Tuple.Create(t.Item1, t.Item2.Left);
					})
				.Retry();
		}
	}
}