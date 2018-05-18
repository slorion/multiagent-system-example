using System;

namespace QbservableProvider
{
	public static class Observable3
	{
		/* Observable.Remotable cannot be used within a sandboxed AppDomain because it requires the RemotingConfiguration permission, 
		 * but it doesn't assert it and it can't be asserted by user code because the assertion must not occur around the call to Subscribe;
		 * otherwise, clients would be able to perform remoting configuration within queries.  Adding this permission to the granted 
		 * permission set for the AppDomain would also mean that clients would be able to perform remoting configuration within queries.
		 * 
		 * To solve this problem, RemotableWithoutConfiguration avoids this permission.
		 */
		public static IObservable<TSource> RemotableWithoutConfiguration<TSource>(this IObservable<TSource> observable)
		{
			return new SerializableObservable<TSource>(new RemotableObservable<TSource>(observable));
		}

		private sealed class RemotableObservable<T> : MarshalByRefObject, IObservable<T>
		{
			private readonly IObservable<T> observable;

			public RemotableObservable(IObservable<T> observable)
			{
				this.observable = observable;
			}

			public override object InitializeLifetimeService()
			{
				return null;
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				return new RemotableSubscription(observable.Subscribe(observer));
			}

			private sealed class RemotableSubscription : MarshalByRefObject, IDisposable
			{
				private readonly IDisposable disposable;

				public RemotableSubscription(IDisposable disposable)
				{
					this.disposable = disposable;
				}

				public override object InitializeLifetimeService()
				{
					return null;
				}

				public void Dispose()
				{
					disposable.Dispose();
				}
			}
		}

		[Serializable]
		private sealed class SerializableObservable<T> : IObservable<T>
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Justification = "MarshalByRefObject")]
			private readonly RemotableObservable<T> observable;

			public SerializableObservable(RemotableObservable<T> observable)
			{
				this.observable = observable;
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				return observable.Subscribe(new RemotableObserver<T>(observer));
			}
		}

		private sealed class RemotableObserver<T> : MarshalByRefObject, IObserver<T>
		{
			private readonly IObserver<T> observer;

			public RemotableObserver(IObserver<T> observer)
			{
				this.observer = observer;
			}

			public override object InitializeLifetimeService()
			{
				return null;
			}

			public void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public void OnError(Exception error)
			{
				observer.OnError(error);
			}

			public void OnCompleted()
			{
				observer.OnCompleted();
			}
		}
	}
}