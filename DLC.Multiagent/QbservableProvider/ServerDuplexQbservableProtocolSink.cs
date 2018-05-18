using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Threading;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	public abstract class ServerDuplexQbservableProtocolSink<TMessage> : QbservableProtocolSink<TMessage>, IServerDuplexQbservableProtocolSink
		where TMessage : IProtocolMessage
	{
		private readonly ConcurrentDictionary<DuplexCallbackId, Tuple<Action<object>, Action<Exception>>> invokeCallbacks = new ConcurrentDictionary<DuplexCallbackId, Tuple<Action<object>, Action<Exception>>>();
		private readonly ConcurrentDictionary<DuplexCallbackId, Tuple<Action<object>, Action<Exception>>> enumeratorCallbacks = new ConcurrentDictionary<DuplexCallbackId, Tuple<Action<object>, Action<Exception>>>();
		private readonly ConcurrentDictionary<DuplexCallbackId, Tuple<Action<object>, Action<Exception>, Action, Action<int>>> observableCallbacks = new ConcurrentDictionary<DuplexCallbackId, Tuple<Action<object>, Action<Exception>, Action, Action<int>>>();
		private readonly Dictionary<DuplexCallbackId, int?> subscriptions = new Dictionary<DuplexCallbackId, int?>();
		private int lastCallbackId;
		private int lastEnumeratorId;
		private int lastObservableId;

		public DuplexCallbackId RegisterInvokeCallback(int clientId, Action<object> callback, Action<Exception> onError)
		{
			var serverId = Interlocked.Increment(ref lastCallbackId);

			var id = new DuplexCallbackId(clientId, serverId);

			if (!invokeCallbacks.TryAdd(id, Tuple.Create(callback, onError)))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexIdForInvoke);
			}

			return id;
		}

		public DuplexCallbackId RegisterEnumeratorCallback(int clientId, Action<object> callback, Action<Exception> onError)
		{
			var serverId = Interlocked.Increment(ref lastEnumeratorId);

			var id = new DuplexCallbackId(clientId, serverId);

			if (!enumeratorCallbacks.TryAdd(id, Tuple.Create(callback, onError)))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexIdForEnumerator);
			}

			return id;
		}

		public Tuple<DuplexCallbackId, IDisposable> RegisterObservableCallbacks(int clientId, Action<object> onNext, Action<Exception> onError, Action onCompleted, Action<int> dispose)
		{
			var serverId = Interlocked.Increment(ref lastObservableId);

			var id = new DuplexCallbackId(clientId, serverId);

			var actions = Tuple.Create(onNext, onError, onCompleted, dispose);

			if (!observableCallbacks.TryAdd(id, actions))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexIdForObservable);
			}

			return Tuple.Create(
				id,
				Disposable.Create(() =>
				{
					lock (actions)
					{
						int? clientSubscriptionId = null;

						if (TryGetOrAddSubscriptionOneTime(id, ref clientSubscriptionId))
						{
							Contract.Assume(clientSubscriptionId.HasValue);		// Disposable.Create ensures that this code only runs once

							actions.Item4(clientSubscriptionId.Value);
						}
					}
				}));
		}

		private Tuple<Action<object>, Action<Exception>> GetInvokeCallbacks(DuplexCallbackId id)
		{
			Tuple<Action<object>, Action<Exception>> actions;

			if (!invokeCallbacks.TryGetValue(id, out actions))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexIdForInvoke);
			}

			return actions;
		}

		private Tuple<Action<object>, Action<Exception>> GetEnumeratorCallbacks(DuplexCallbackId id)
		{
			Tuple<Action<object>, Action<Exception>> actions;

			if (!enumeratorCallbacks.TryGetValue(id, out actions))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexIdForEnumerator);
			}

			return actions;
		}

		private bool TryGetOrAddSubscriptionOneTime(DuplexCallbackId id, ref int? clientSubscriptionId)
		{
			int? s;
			if (subscriptions.TryGetValue(id, out s))
			{
				subscriptions.Remove(id);

				clientSubscriptionId = s;
				return true;
			}
			else
			{
				subscriptions.Add(id, clientSubscriptionId);
			}

			return false;
		}

		private void TryInvokeObservableCallback(
			DuplexCallbackId id,
			Action<Tuple<Action<object>, Action<Exception>, Action, Action<int>>> action)
		{
			Tuple<Action<object>, Action<Exception>, Action, Action<int>> callbacks;

			if (observableCallbacks.TryGetValue(id, out callbacks))
			{
				action(callbacks);
			}

			/* It's acceptable for the callbacks to be missing due to a race condition between the
			 * client sending notifications and the server disposing of the subscription, which causes
			 * the callbacks to be removed.
			 */
		}

		public abstract object Invoke(int clientId, object[] arguments);

		public abstract IDisposable Subscribe(int clientId, Action<object> onNext, Action<Exception> onError, Action onCompleted);

		public abstract int GetEnumerator(int clientId);

		public abstract Tuple<bool, object> MoveNext(int enumeratorId);

		public abstract void ResetEnumerator(int enumeratorId);

		public abstract void DisposeEnumerator(int enumeratorId);

		protected void HandleResponse(DuplexCallbackId id, object value)
		{
			GetInvokeCallbacks(id).Item1(value);
		}

		protected void HandleErrorResponse(DuplexCallbackId id, Exception error)
		{
			GetInvokeCallbacks(id).Item2(error);
		}

		protected void HandleGetEnumeratorResponse(DuplexCallbackId id, int clientEnumeratorId)
		{
			HandleResponse(id, clientEnumeratorId);
		}

		protected void HandleGetEnumeratorErrorResponse(DuplexCallbackId id, Exception error)
		{
			HandleErrorResponse(id, error);
		}

		protected void HandleEnumeratorResponse(DuplexCallbackId id, Tuple<bool, object> result)
		{
			GetEnumeratorCallbacks(id).Item1(result);
		}

		protected void HandleEnumeratorErrorResponse(DuplexCallbackId id, Exception error)
		{
			GetEnumeratorCallbacks(id).Item2(error);
		}

		protected void HandleSubscribeResponse(DuplexCallbackId id, int clientSubscriptionId)
		{
			Tuple<Action<object>, Action<Exception>, Action, Action<int>> actions;

			if (!observableCallbacks.TryGetValue(id, out actions))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexIdForObservable);
			}

			lock (actions)
			{
				int? inputOnly = clientSubscriptionId;
				if (TryGetOrAddSubscriptionOneTime(id, ref inputOnly))
				{
					subscriptions.Remove(id);

					Tuple<Action<object>, Action<Exception>, Action, Action<int>> ignored;
					observableCallbacks.TryRemove(id, out ignored);

					actions.Item4(clientSubscriptionId);
				}
			}
		}

		protected void HandleOnNext(DuplexCallbackId id, object result)
		{
			TryInvokeObservableCallback(id, actions => actions.Item1(result));
		}

		protected void HandleOnCompleted(DuplexCallbackId id)
		{
			TryInvokeObservableCallback(id, actions => actions.Item3());
		}

		protected void HandleOnError(DuplexCallbackId id, Exception error)
		{
			TryInvokeObservableCallback(id, actions => actions.Item2(error));
		}
	}
}