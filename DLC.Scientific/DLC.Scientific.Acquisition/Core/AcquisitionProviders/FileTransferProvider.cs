using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class FileTransferProvider
		: AcquisitionProvider<FileTransferData>
	{
		private readonly BehaviorSubject<bool> _isTransferringSubject = new BehaviorSubject<bool>(false);

		public bool IsTransferring { get { return _isTransferringSubject.Value; } }
		public IObservable<bool> IsTransferringDataSource { get { return _isTransferringSubject.DistinctUntilChanged(); } }

		public void StartTransferring()
		{
			StartTransferringCore();
			_isTransferringSubject.OnNext(true);
		}

		public Task StopTransferring()
		{
			return StopTransferringCore()
				.ContinueWith(t => _isTransferringSubject.OnNext(false), TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		protected virtual void StartTransferringCore() { }
		protected virtual Task StopTransferringCore() { return Task.FromResult(0); }
	}
}