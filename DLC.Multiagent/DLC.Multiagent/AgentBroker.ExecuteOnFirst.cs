using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public Task<ExecutionResult> TryExecuteOnFirst<TAgent>(Action<TAgent> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, CancellationToken? ct = null, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnFirst<TAgent>(a => Task.Run(() => operation(a), ct ?? CancellationToken.None), scope, ct, ignoreAgentState);
		}

		public async Task<ExecutionResult> TryExecuteOnFirst<TAgent>(Func<TAgent, Task> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, CancellationToken? ct = null, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return await TryExecuteOnFirst<TAgent, object>(async a => { await operation(a).ConfigureAwait(false); return (object) null; }, scope, ct, ignoreAgentState).ConfigureAwait(false);
		}

		public Task<ExecutionResult<TResult>> TryExecuteOnFirst<TAgent, TResult>(Func<TAgent, TResult> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, CancellationToken? ct = null, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnFirst<TAgent, TResult>(a => Task.Run(() => operation(a), ct ?? CancellationToken.None), scope, ct, ignoreAgentState);
		}

		public async Task<ExecutionResult<TResult>> TryExecuteOnFirst<TAgent, TResult>(Func<TAgent, Task<TResult>> operation, ExecutionScopeOptions scope = ExecutionScopeOptions.All, CancellationToken? ct = null, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			if (operation == null) throw new ArgumentNullException("operation");

			ct = ct ?? CancellationToken.None;

			ExecutionResult<TResult> result = null;
			foreach (var agentInfo in GetAgentInfos<TAgent>(scope).Where(info => ignoreAgentState || (info.IsReachable && info.LastKnownState == AgentState.Activated)))
			{
				if (ct.Value.IsCancellationRequested)
				{
					result = new ExecutionResult<TResult> { AgentId = agentInfo.AgentId, IsCanceled = true };
					break;
				}
				else
				{
					result = await TryExecuteOnOne(agentInfo.AgentId, operation, ignoreAgentState).ConfigureAwait(false);

					if (result.IsSuccessful)
						break;
				}
			}

			if (result == null)
				result = new ExecutionResult<TResult> { Exception = new InvalidOperationException(string.Format("An agent that implements the contract '{0}' was not found.", typeof(TAgent).AssemblyQualifiedName)) };

			return result;
		}
	}
}