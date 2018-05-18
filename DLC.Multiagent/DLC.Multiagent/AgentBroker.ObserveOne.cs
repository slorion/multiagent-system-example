using DLC.Multiagent.Logging;
using NLog.Fluent;
using QbservableProvider;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;

namespace DLC.Multiagent
{
	partial class AgentBroker
	{
		public IObservable<T> ObserveOne<T>(string agentId, string propertyName, bool ignoreAgentState = false)
		{
			return ObserveOneUnsafe<T>(agentId, propertyName, ignoreAgentState).SelectLeft(left => left);
		}

		private IObservable<Either<T, Exception>> ObserveOneUnsafe<T>(string agentId, string propertyName, bool ignoreAgentState = false, bool ignoreUnreachable = false, TimeSpan? timeout = null)
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");

			var observable = Observable.Defer(
				() =>
				{
					AgentInformation agentInfo;
					if (!TryGetAgentInformation(agentId, out agentInfo))
						throw new InvalidOperationException(string.Format("Agent '{0}' cannot be found.", agentId));

					if (agentInfo.IsRecycled)
						throw new InvalidOperationException(string.Format("Agent '{0}' has been recycled.", agentId));
					else if (!ignoreUnreachable && !agentInfo.IsReachable)
						throw new InvalidOperationException(string.Format("Agent '{0}' has been disconnected.", agentId));

					if (agentInfo.IsLocal)
					{
						var local = (LocalAgentInformation) agentInfo;
						var dataSource = (IObservable<T>) local.Agent.GetType().GetProperty(propertyName).GetValue(local.Agent);

						// if agent becomes invalid, manually create an error to invoke Retry wrapper added at the end of this method
						return
							local.Agent.StateDataSource
								.Select(
									state =>
									{
										if (state == AgentState.Disposed)
											return Notification.CreateOnError<T>(new InvalidOperationException(string.Format("Agent '{0}' has been disposed.", local.AgentId)));
										else if (!ignoreAgentState && state != AgentState.Activated)
											return Notification.CreateOnError<T>(new InvalidOperationException(string.Format("Agent '{0}' is not active.", local.AgentId)));
										else
											return Notification.CreateOnNext(default(T));
									})
								.Dematerialize()
								.IgnoreElements()
								.Merge(dataSource);
					}
					else
					{
						var remote = (RemoteAgentInformation) agentInfo;
						var peerAgentId = GetAgentId(remote.PeerNode, PeerCommunicationAgent.Configuration.Name);

						var datasource =
							Observable.FromAsync(() => TryExecuteOnOneUnsafe<IPeerCommunicationAgent, int>(peerAgentId, a => a.EnsureObservableListening(remote.AgentId, propertyName), ignoreAgentState: false, ignoreUnreachable: ignoreUnreachable))
								.SelectMany(
									portResult =>
									{
										if (!portResult.IsSuccessful)
											throw portResult.Exception ?? (portResult.IsCanceled ? (Exception) new OperationCanceledException() : new InvalidOperationException(string.Format("An unknown error occurred while getting the Rx port via agent '{0}'.", portResult.AgentId)));
										else
										{
											var knownTypes = new List<Type>();
											knownTypes.Add(typeof(T));
											if (typeof(T).IsGenericType)
												knownTypes.AddRange(typeof(T).GenericTypeArguments);

											var client = new QbservableTcpClient<T>(new IPEndPoint(remote.PeerNode.Host, portResult.Result), knownTypes.ToArray());
											return client.Query();
										}
									});

						return
							this.AgentDataSource
								.Where(info => string.Equals(info.AgentId, agentId, StringComparison.InvariantCulture))
								.Select(
									info =>
									{
										// agent deconnection is already managed by QbservableTcpClient
										if (info.LastKnownState == AgentState.Disposed)
											return Notification.CreateOnError<T>(new InvalidOperationException(string.Format("Agent '{0}' has been disposed.", info.AgentId)));
										else if (!ignoreAgentState && info.LastKnownState != AgentState.Activated)
											return Notification.CreateOnError<T>(new InvalidOperationException(string.Format("Agent '{0}' is not active.", info.AgentId)));
										else
											return Notification.CreateOnNext(default(T));
									})
								.Dematerialize()
								.IgnoreElements()
								.Merge(datasource);
					}
				});

			// if an error occurs, try to reconnect after retryCount * MinOperationRetryDelay, up to MaxOperationRetryDelay
			return observable
				.Retry(
					int.MaxValue,
					(ex, retryCount) =>
					{
						Log.Trace().Message("Could not connect to the observable related to property '{0}'.", propertyName).Exception(ex).WithAgent(agentId).Write();

						if (timeout != null)
							return timeout.Value;
						else
							return TimeSpan.FromMilliseconds(Math.Min(retryCount * this.MinOperationRetryDelay.TotalMilliseconds, this.MaxOperationRetryDelay.TotalMilliseconds));
					})
				.TakeWhile(_ => this.State == ServiceState.Started);
		}
	}
}