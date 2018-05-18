using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace QbservableProvider
{
	[Serializable]
	internal sealed class DuplexCallbackObservable<T> : DuplexCallback, IObservable<T>
	{
		public DuplexCallbackObservable(int id)
			: base(id)
		{
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			/* In testing, the observer permanently blocked incoming data from the client unless concurrency was introduced.
			 * The order of events were as follows: 
			 * 
			 * 1. The server received an OnNext notification from an I/O completion port.
			 * 2. The server pushed the value to the observer passed into this method, without introducing concurrency.
			 * 3. The query provider continued executing the serialized query on the current thread.
			 * 4. The query at this point required a synchronous invocation to a client-side member (i.e., duplex enabled).
			 * 5. The server sent the new invocation to the client and then blocked the current thread waiting for an async response.
			 * 
			 * Since the current thread was an I/O completion port (received for OnNext), it seems that blocking it prevented any 
			 * further data from being received, even via the Stream.AsyncRead method.  Apparently the only solution is to ensure 
			 * that observable callbacks occur on pooled threads to prevent I/O completion ports from inadvertantly being blocked.
			 */
			var scheduler = TaskPoolScheduler.Default;

			Action<Action> tryExecute =
				action =>
				{
					scheduler.Schedule(() =>
					{
						try
						{
							action();
						}
						catch (Exception ex)
						{
							Protocol.CancelAllCommunication(ex);
						}
					});
				};

			try
			{
				return Sink.Subscribe(
					Id,
					value => tryExecute(() => observer.OnNext((T) value)),
					ex => tryExecute(() => observer.OnError(ex)),
					() => tryExecute(observer.OnCompleted));
			}
			catch (Exception ex)
			{
				Protocol.CancelAllCommunication(ex);

				return Disposable.Empty;
			}
		}
	}
}