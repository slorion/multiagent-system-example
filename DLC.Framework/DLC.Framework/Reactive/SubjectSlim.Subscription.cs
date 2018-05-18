using System;

namespace DLC.Framework.Reactive
{
	partial class SubjectSlim<T>
	{
		class Subscription<T2>
			: IDisposable
		{
			private SubjectSlim<T2> _subject;
			private IObserver<T2> _observer;

			public Subscription(SubjectSlim<T2> subject, IObserver<T2> observer)
			{
				if (subject == null) throw new ArgumentNullException("subject");
				if (observer == null) throw new ArgumentNullException("observer");

				_subject = subject;
				_observer = observer;
			}

			#region IDisposable members

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				var subject = _subject;
				var observer = _observer;

				if (subject != null && observer != null)
					subject.Unsubscribe(observer);

				_subject = null;
				_observer = null;
			}

			~Subscription()
			{
				Dispose(false);
			}

			#endregion
		}
	}
}