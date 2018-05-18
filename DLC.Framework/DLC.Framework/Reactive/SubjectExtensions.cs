using System;
using System.Reactive.Subjects;

namespace DLC.Framework.Reactive
{
	public static class SubjectExtensions
	{
		public static DeferredSubject<T> ToDeferred<T>(this ISubject<T> subject)
		{
			if (subject == null) throw new ArgumentNullException("subject");

			return new DeferredSubject<T>(subject);
		}
	}
}