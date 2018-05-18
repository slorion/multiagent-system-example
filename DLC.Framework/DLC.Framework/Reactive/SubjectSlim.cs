using NLog.Fluent;
using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace DLC.Framework.Reactive
{
	public partial class SubjectSlim<T>
		: ISubject<T>
	{
		private readonly object _lock = new object();
		private ImmutableList<IObserver<T>> _observers = ImmutableList<IObserver<T>>.Empty;

		public bool IsCompleted { get; private set; }
		public Exception Error { get; private set; }

		public void OnNext(T value)
		{
			CheckStillActive();

			OnNextCore(value);

			var observers = _observers;
			foreach (var obs in observers)
			{
				try { obs.OnNext(value); }
				catch (Exception ex) { Log.Trace().Message("Error when calling OnNext on observer '{0}'.", obs).Exception(ex).Write(); }
			}
		}
		protected virtual void OnNextCore(T value) { }

		public void OnCompleted()
		{
			CheckStillActive();

			this.IsCompleted = true;

			OnCompletedCore();

			var observers = _observers;
			foreach (var obs in observers)
			{
				try { obs.OnCompleted(); }
				catch (Exception ex) { Log.Trace().Message("Error when calling OnCompleted on observer '{0}'.", obs).Exception(ex).Write(); }
			}

			UnsubscribeAll();
		}
		protected virtual void OnCompletedCore() { }

		public void OnError(Exception error)
		{
			if (error == null) throw new ArgumentNullException("error");

			CheckStillActive();

			this.Error = error;

			OnErrorCore();

			var observers = _observers;
			foreach (var obs in observers)
			{
				try { obs.OnError(error); }
				catch (Exception ex) { Log.Trace().Message("Error when calling OnError on observer '{0}'.", obs).Exception(ex).Write(); }
			}

			UnsubscribeAll();
		}
		protected virtual void OnErrorCore() { }

		protected virtual void OnSubscriptionAdded(IObserver<T> observer) { }
		protected virtual void OnSubscriptionRemoved(IObserver<T> observer) { }

		public IDisposable Subscribe(IObserver<T> observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");

			if (this.IsCompleted || this.Error != null)
			{
				OnSubscriptionAdded(observer);
				return Disposable.Empty;
			}
			else
			{
				lock (_lock) { _observers = _observers.Add(observer); }
				OnSubscriptionAdded(observer);
				return new Subscription<T>(this, observer);
			}
		}

		private void Unsubscribe(IObserver<T> observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");

			lock (_lock) { _observers = _observers.Remove(observer); }
			OnSubscriptionRemoved(observer);
		}

		private void UnsubscribeAll()
		{
			var observers = _observers;
			lock (_lock) { _observers = ImmutableList<IObserver<T>>.Empty; }

			foreach (var obs in observers)
				OnSubscriptionRemoved(obs);
		}

		protected void CheckStillActive()
		{
			if (this.IsCompleted)
				throw new InvalidOperationException("Sequence already completed.");

			if (this.Error != null)
				throw new InvalidOperationException("Sequence already completed with an error.");
		}
	}
}