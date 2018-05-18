using NLog.Fluent;
using System;

namespace DLC.Framework.Reactive
{
	public class BehaviorSubjectSlim<T>
		: SubjectSlim<T>
	{
		private bool _hasValue;
		private T _value;

		public BehaviorSubjectSlim()
		{
		}

		public BehaviorSubjectSlim(T value)
			: this()
		{
			_hasValue = true;
			_value = value;
		}

		public T Value { get { return _value; } }

		protected override void OnNextCore(T value)
		{
			base.OnNextCore(value);
			_value = value;
			_hasValue = true;
		}

		protected override void OnSubscriptionAdded(IObserver<T> observer)
		{
			base.OnSubscriptionAdded(observer);

			if (this.IsCompleted)
			{
				try { observer.OnCompleted(); }
				catch (Exception ex) { Log.Trace().Message("Error when calling OnCompleted on observer '{0}'.", observer).Exception(ex).Write(); }
			}
			else if (this.Error != null)
			{
				try { observer.OnError(this.Error); }
				catch (Exception ex) { Log.Trace().Message("Error when calling OnError on observer '{0}'.", observer).Exception(ex).Write(); }
			}
			else if (_hasValue)
			{
				try { observer.OnNext(this.Value); }
				catch (Exception ex) { Log.Trace().Message("Error when calling OnNext on observer '{0}'.", observer).Exception(ex).Write(); }
			}
		}
	}
}