using DLC.Framework.Reactive;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public sealed class ManualProvider
		: ManualProvider<ProviderData>
	{
	}

	public class ManualProvider<TData>
		: AcquisitionProvider<TData>
		where TData : ProviderData
	{
		private readonly SubjectSlim<TData> _datasourceSubject = new SubjectSlim<TData>();

		protected override Task<IObservable<TData>> InitializeCore()
		{
			return Task.FromResult(_datasourceSubject.AsObservable());
		}

		public void OnNext(TData value) { _datasourceSubject.OnNext(value); }
		public void OnError(Exception error) { _datasourceSubject.OnError(error); }
		public void OnCompleted() { _datasourceSubject.OnCompleted(); }
	}
}