using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	public abstract class ClientDuplexQbservableProtocolSink<TMessage> : QbservableProtocolSink<TMessage>, IClientDuplexQbservableProtocolSink
		where TMessage : IProtocolMessage
	{
		private readonly ConcurrentDictionary<int, Func<object[], object>> invokeCallbacks = new ConcurrentDictionary<int, Func<object[], object>>();
		private readonly ConcurrentDictionary<int, Func<IEnumerator>> enumerableCallbacks = new ConcurrentDictionary<int, Func<IEnumerator>>();
		private readonly ConcurrentDictionary<int, Func<int, IDisposable>> obsevableCallbacks = new ConcurrentDictionary<int, Func<int, IDisposable>>();
		private readonly ConcurrentDictionary<int, IEnumerator> enumerators = new ConcurrentDictionary<int, IEnumerator>();
		private readonly ConcurrentDictionary<int, IDisposable> subscriptions = new ConcurrentDictionary<int, IDisposable>();
		private int lastCallbackId;
		private int lastObservableId;
		private int lastSubscriptionId;
		private int lastEnumerableId;
		private int lastEnumeratorId;

		public int RegisterInvokeCallback(Func<object[], object> callback)
		{
			var id = Interlocked.Increment(ref lastCallbackId);

			if (!invokeCallbacks.TryAdd(id, callback))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexId);
			}

			return id;
		}

		public int RegisterEnumerableCallback(Func<IEnumerator> getEnumerator)
		{
			var id = Interlocked.Increment(ref lastEnumerableId);

			if (!enumerableCallbacks.TryAdd(id, getEnumerator))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexId);
			}

			return id;
		}

		public int RegisterObservableCallback(Func<int, IDisposable> subscribe)
		{
			var id = Interlocked.Increment(ref lastObservableId);

			if (!obsevableCallbacks.TryAdd(id, subscribe))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexId);
			}

			return id;
		}

		private int RegisterSubscription(IDisposable subscription)
		{
			var id = Interlocked.Increment(ref lastSubscriptionId);

			if (!subscriptions.TryAdd(id, subscription))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexId);
			}

			return id;
		}

		private int RegisterEnumerator(IEnumerator enumerator)
		{
			var id = Interlocked.Increment(ref lastEnumeratorId);

			if (!enumerators.TryAdd(id, enumerator))
			{
				throw new InvalidOperationException(Errors.ProtocolDuplicateDuplexId);
			}

			return id;
		}

		private Func<IEnumerator> GetEnumerable(int clientId)
		{
			Func<IEnumerator> enumerable;

			if (!enumerableCallbacks.TryGetValue(clientId, out enumerable))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexId);
			}

			return enumerable;
		}

		private IEnumerator GetEnumerator(int enumeratorId)
		{
			IEnumerator enumerator;

			if (enumerators.TryGetValue(enumeratorId, out enumerator))
			{
				return enumerator;
			}

			// The enumerator may be missing if Disposed has been called or if MoveNext is called again after it has already returned false.
			return null;
		}

		protected void Invoke(DuplexCallbackId id, object[] arguments)
		{
			Func<object[], object> callback;

			if (!invokeCallbacks.TryGetValue(id.ClientId, out callback))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexId);
			}

			object result;
			try
			{
				result = callback(arguments);
			}
			catch (Exception ex)
			{
				SendError(id, ex);
				return;
			}

			SendResponse(id, result);
		}

		protected void Subscribe(DuplexCallbackId id)
		{
			Func<int, IDisposable> subscribe;

			if (!obsevableCallbacks.TryGetValue(id.ClientId, out subscribe))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexId);
			}

			var subscription = new SingleAssignmentDisposable();

			var subscriptionId = RegisterSubscription(subscription);

			try
			{
				subscription.Disposable = subscribe(id.ServerId);
			}
			catch (Exception ex)
			{
				SendOnError(id, ex);
				return;
			}

			SendSubscribeResponse(id, subscriptionId);
		}

		protected void DisposeSubscription(int subscriptionId)
		{
			IDisposable subscription;

			if (!subscriptions.TryGetValue(subscriptionId, out subscription))
			{
				throw new InvalidOperationException(Errors.ProtocolInvalidDuplexId);
			}

			subscription.Dispose();
		}

		protected void GetEnumerator(DuplexCallbackId id)
		{
			var enumerable = GetEnumerable(id.ClientId);

			IEnumerator enumerator;
			try
			{
				enumerator = enumerable();
			}
			catch (Exception ex)
			{
				SendGetEnumeratorError(id, ex);
				return;
			}

			var enumeratorId = RegisterEnumerator(enumerator);

			SendGetEnumeratorResponse(id, enumeratorId);
		}

		protected void MoveNext(DuplexCallbackId id)
		{
			var enumerator = GetEnumerator(id.ClientId);

			object current;
			bool result;

			try
			{
				if (enumerator != null && enumerator.MoveNext())
				{
					result = true;
					current = enumerator.Current;
				}
				else
				{
					result = false;
					current = null;
				}
			}
			catch (Exception ex)
			{
				SendEnumeratorError(id, ex);
				return;
			}

			SendEnumeratorResponse(id, result, current);
		}

		protected void ResetEnumerator(DuplexCallbackId id)
		{
			var enumerator = GetEnumerator(id.ClientId);

			try
			{
				enumerator.Reset();
			}
			catch (Exception ex)
			{
				SendEnumeratorError(id, ex);
			}
		}

		protected void DisposeEnumerator(int enumeratorId)
		{
			var disposable = GetEnumerator(enumeratorId) as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public abstract void SendOnNext(DuplexCallbackId id, object value);

		public abstract void SendOnError(DuplexCallbackId id, Exception error);

		public abstract void SendOnCompleted(DuplexCallbackId id);

		protected abstract void SendResponse(DuplexCallbackId id, object result);

		protected abstract void SendError(DuplexCallbackId id, Exception error);

		protected abstract void SendSubscribeResponse(DuplexCallbackId id, int clientSubscriptionId);

		protected abstract void SendGetEnumeratorResponse(DuplexCallbackId id, int clientEnumeratorId);

		protected abstract void SendGetEnumeratorError(DuplexCallbackId id, Exception error);

		protected abstract void SendEnumeratorResponse(DuplexCallbackId id, bool result, object current);

		protected abstract void SendEnumeratorError(DuplexCallbackId id, Exception error);
	}
}