using System;

namespace QbservableProvider
{
	public interface IServerDuplexQbservableProtocolSink
	{
		DuplexCallbackId RegisterInvokeCallback(int clientId, Action<object> callback, Action<Exception> onError);

		DuplexCallbackId RegisterEnumeratorCallback(int clientId, Action<object> callback, Action<Exception> onError);

		Tuple<DuplexCallbackId, IDisposable> RegisterObservableCallbacks(int clientId, Action<object> onNext, Action<Exception> onError, Action onCompleted, Action<int> dispose);

		object Invoke(int clientId, object[] arguments);

		IDisposable Subscribe(int clientId, Action<object> onNext, Action<Exception> onError, Action onCompleted);

		int GetEnumerator(int clientId);

		Tuple<bool, object> MoveNext(int enumeratorId);

		void ResetEnumerator(int enumeratorId);

		void DisposeEnumerator(int enumeratorId);
	}
}