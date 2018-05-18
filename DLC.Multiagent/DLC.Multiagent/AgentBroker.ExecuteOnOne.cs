using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public async Task<ExecutionResult> TryExecuteOnOne<TAgent>(string agentId, Action<TAgent> operation, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return (ExecutionResult) await TryExecuteOnOne<TAgent, object>(agentId, a => Task.Run<object>(() => { operation(a); return (object) null; }), ignoreAgentState).ConfigureAwait(false);
		}

		public async Task<ExecutionResult> TryExecuteOnOne<TAgent>(string agentId, Func<TAgent, Task> operation, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return await TryExecuteOnOne<TAgent, object>(agentId, async a => { await operation(a).ConfigureAwait(false); return (object) null; }, ignoreAgentState).ConfigureAwait(false);
		}

		public Task<ExecutionResult<TResult>> TryExecuteOnOne<TAgent, TResult>(string agentId, Func<TAgent, TResult> operation, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnOne<TAgent, TResult>(agentId, a => Task.Run<TResult>(() => operation(a)), ignoreAgentState);
		}

		public Task<ExecutionResult<TResult>> TryExecuteOnOne<TAgent, TResult>(string agentId, Func<TAgent, Task<TResult>> operation, bool ignoreAgentState = false)
			where TAgent : IAgent
		{
			return TryExecuteOnOneUnsafe<TAgent, TResult>(agentId, operation, ignoreAgentState, ignoreUnreachable: false);
		}

		private async Task<ExecutionResult> TryExecuteOnOneUnsafe<TAgent>(string agentId, Action<TAgent> operation, bool ignoreAgentState = false, bool ignoreUnreachable = false)
			where TAgent : IAgent
		{
			return (ExecutionResult) await TryExecuteOnOneUnsafe<TAgent, object>(agentId, a => Task.Run<object>(() => { operation(a); return (object) null; }), ignoreAgentState, ignoreUnreachable).ConfigureAwait(false);
		}

		private async Task<ExecutionResult> TryExecuteOnOneUnsafe<TAgent>(string agentId, Func<TAgent, Task> operation, bool ignoreAgentState = false, bool ignoreUnreachable = false)
			where TAgent : IAgent
		{
			return (ExecutionResult) await TryExecuteOnOneUnsafe<TAgent, object>(agentId, a => operation(a).ContinueWith(t => (object) null), ignoreAgentState, ignoreUnreachable).ConfigureAwait(false);
		}

		private Task<ExecutionResult<TResult>> TryExecuteOnOneUnsafe<TAgent, TResult>(string agentId, Func<TAgent, TResult> operation, bool ignoreAgentState = false, bool ignoreUnreachable = false)
			where TAgent : IAgent
		{
			return TryExecuteOnOneUnsafe<TAgent, TResult>(agentId, a => Task.Run<TResult>(() => operation(a)), ignoreAgentState, ignoreUnreachable);
		}

		private async Task<ExecutionResult<TResult>> TryExecuteOnOneUnsafe<TAgent, TResult>(string agentId, Func<TAgent, Task<TResult>> operation, bool ignoreAgentState = false, bool ignoreUnreachable = false)
			where TAgent : IAgent
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (operation == null) throw new ArgumentNullException("operation");

			var getAgentResult = this.TryGetAgent<TAgent>(agentId);

			if (getAgentResult.Item1 == TryGetAgentResult.Success
				|| (ignoreAgentState && getAgentResult.Item1 == TryGetAgentResult.NotActivated)
				|| (ignoreUnreachable && getAgentResult.Item1 == TryGetAgentResult.Unreachable))
			{
				try
				{
					if (getAgentResult.Item1 == TryGetAgentResult.Unreachable)
					{
						var ping = new Ping();
						var pingReply = await ping.SendPingAsync(getAgentResult.Item2.PeerNode.Host, this.Configuration.HeartbeatFrequencyInMs).ConfigureAwait(false);

						if (pingReply.Status != IPStatus.Success)
							return new ExecutionResult<TResult> { AgentId = agentId, Exception = new InvalidOperationException(string.Format("Communication with agent '{0}' cannot be established.", agentId)) };
					}

					var result = await operation((TAgent) getAgentResult.Item3).ConfigureAwait(false);
					return new ExecutionResult<TResult> { AgentId = agentId, Result = result };
				}
				catch (TaskCanceledException ex)
				{
					return new ExecutionResult<TResult> { AgentId = agentId, IsCanceled = true, Exception = ex };
				}
				catch (Exception ex)
				{
					return new ExecutionResult<TResult> { AgentId = agentId, Exception = ex };
				}
			}
			else if (getAgentResult.Item1 == TryGetAgentResult.NotActivated)
				return new ExecutionResult<TResult> { AgentId = agentId, Exception = new InvalidOperationException(string.Format("Agent '{0}' is not active.", agentId)) };
			else if (getAgentResult.Item1 == TryGetAgentResult.Unreachable)
				return new ExecutionResult<TResult> { AgentId = agentId, Exception = new InvalidOperationException(string.Format("Communication with agent '{0}' cannot be established.", agentId)) };
			else if (getAgentResult.Item1 == TryGetAgentResult.NotFound)
				return new ExecutionResult<TResult> { AgentId = agentId, Exception = new InvalidOperationException(string.Format("Agent '{0}' cannot be found.", agentId)) };
			else if (getAgentResult.Item1 == TryGetAgentResult.ContractNotImplemented)
				return new ExecutionResult<TResult> { AgentId = agentId, Exception = new InvalidOperationException(string.Format("Agent '{0}' does not implement the contract '{1}'.", agentId, typeof(TAgent).AssemblyQualifiedName)) };
			else
				return new ExecutionResult<TResult> { AgentId = agentId, Exception = new InvalidOperationException(string.Format("The operation on agent '{0}' has failed with an unknown reason.", agentId)) };
		}
	}
}