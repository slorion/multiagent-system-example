using NLog.Fluent;
using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DLC.Framework.Reactive
{
	public class DeferredSubject<T>
		: ISubject<T>, IDisposable
	{
		private readonly BlockingCollection<Notification<T>> _queue = new BlockingCollection<Notification<T>>();
		private readonly ISubject<T> _inner;
		private readonly Task _listeningTask;

		public DeferredSubject(ISubject<T> subject)
		{
			if (subject == null) throw new ArgumentNullException("subject");

			_inner = subject;
			_listeningTask = Task.Run(
				() =>
				{
					try
					{
						foreach (var notification in _queue.GetConsumingEnumerable())
							notification.Accept(_inner);
					}
					finally
					{
						_queue.Dispose();
					}
				});
		}

		public void OnCompleted()
		{
			_queue.Add(Notification.CreateOnCompleted<T>());
			_queue.CompleteAdding();
		}

		public void OnError(Exception error)
		{
			_queue.Add(Notification.CreateOnError<T>(error));
			_queue.CompleteAdding();
		}

		public void OnNext(T value)
		{
			_queue.Add(Notification.CreateOnNext<T>(value));
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			return _inner.Subscribe(observer);
		}

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_queue != null)
			{
				try
				{
					var count = _queue.Count;
					if (count > 0)
						Log.Debug().Message("When Dispose was called, {0} elements were still queued in DeferredSubject ('{1}').", count, _inner.GetType()).Write();

					_queue.CompleteAdding();
				}
				catch (ObjectDisposedException) { }
			}
		}

		~DeferredSubject()
		{
			Log.Warn().Message("Object was not disposed correctly.").Write();
			Dispose(false);
		}

		#endregion
	}
}