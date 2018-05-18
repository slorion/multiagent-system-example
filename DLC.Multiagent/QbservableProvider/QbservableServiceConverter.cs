using System;
using System.Reactive.Linq;

namespace QbservableProvider
{
	// This class must be public otherwise CreateService fails inside of a new AppDomain - see CreateServiceProxy comments
	[Serializable]
	public sealed class QbservableServiceConverter<TSource, TResult>
	{
		private readonly Func<IObservable<TSource>, IObservable<TResult>> service;

		public QbservableServiceConverter(Func<IObservable<TSource>, IObservable<TResult>> service)
		{
			this.service = service;
		}

		public IQbservable<TResult> Convert(IObservable<TSource> request)
		{
			return service(request).AsQbservable();
		}
	}
}