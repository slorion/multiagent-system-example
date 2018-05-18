using System;
using System.Collections;

namespace QbservableProvider
{
	public interface IClientDuplexQbservableProtocolSink
	{
		int RegisterInvokeCallback(Func<object[], object> callback);

		int RegisterEnumerableCallback(Func<IEnumerator> getEnumerator);

		int RegisterObservableCallback(Func<int, IDisposable> subscribe);

		void SendOnNext(DuplexCallbackId id, object value);

		void SendOnError(DuplexCallbackId id, Exception error);

		void SendOnCompleted(DuplexCallbackId id);
	}
}