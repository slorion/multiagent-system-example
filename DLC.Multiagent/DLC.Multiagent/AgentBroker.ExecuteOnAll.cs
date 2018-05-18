using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public IEnumerable<Task<ExecutionResult>> TryExecuteOnAll<TAgent>(Action<TAgent> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, CancellationToken? ct = null, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnAll<TAgent>(a => Task.Run(() => operation(a), ct ?? CancellationToken.None), scope, ignoreAgentState);
		}

		public IEnumerable<Task<ExecutionResult>> TryExecuteOnAll<TAgent>(Func<TAgent, Task> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnAll<TAgent, object>(async a => { await operation(a).ConfigureAwait(false); return (object) null; }, scope, ignoreAgentState)
				.Select(async t => { return (ExecutionResult) await t.ConfigureAwait(false); });
		}

		public IEnumerable<Task<ExecutionResult<TResult>>> TryExecuteOnAll<TAgent, TResult>(Func<TAgent, TResult> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, CancellationToken? ct = null, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnAll<TAgent, TResult>(a => Task.Run(() => operation(a), ct ?? CancellationToken.None), scope, ignoreAgentState);
		}

		public IEnumerable<Task<ExecutionResult<TResult>>> TryExecuteOnAll<TAgent, TResult>(Func<TAgent, Task<TResult>> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (operation == null) throw new ArgumentNullException("operation");

			return this.GetAgentInfos<TAgent>(scope)
				.Where(info => ignoreAgentState || (info.IsReachable && info.LastKnownState == AgentState.Activated))
				.Select(info => TryExecuteOnOne(info.AgentId, operation, ignoreAgentState));
		}
	}
}