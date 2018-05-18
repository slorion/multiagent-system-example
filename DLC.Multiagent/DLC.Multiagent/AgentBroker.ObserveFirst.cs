using System;
using System.Reactive.Linq;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public IObservable<T> ObserveFirst<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return ObserveFirstUnsafe<TAgent, T>(propertyName, scope, ignoreAgentState).SelectLeft(left => left);
		}

		private IObservable<Either<T, Exception>> ObserveFirstUnsafe<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			return ObserveFirstUnsafeWithAgentInfo<TAgent, T>(propertyName, scope, ignoreAgentState)
				.Select(t => t.Item2);
		}

		public IObservable<Tuple<AgentInformation, T>> ObserveFirstWithAgentInfo<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return ObserveFirstUnsafeWithAgentInfo<TAgent, T>(propertyName, scope, ignoreAgentState)
				.Where(t => t.Item2.IsLeft)
				.Select(t => Tuple.Create(t.Item1, t.Item2.Left));
		}

		private IObservable<Tuple<AgentInformation, Either<T, Exception>>> ObserveFirstUnsafeWithAgentInfo<TAgent, T>(string propertyName, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			return ObserveAllUnsafe<TAgent, T>(propertyName, scope, ignoreAgentState)
				.Take(1)
				.SelectMany(t => t.Item2.Select(data => Tuple.Create(t.Item1, data)));
		}
	}
}