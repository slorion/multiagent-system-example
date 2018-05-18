using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public interface IAcquisitionProvider
		: IDisposable
	{
		ProviderState State { get; }
		IObservable<ProviderState> ProviderStateDataSource { get; }

		IObservable<ProviderData> DataSource { get; }
		ProviderData CurrentData { get; }
		long DataReceivedCount { get; }

		bool CanCalibrate { get; }

		Task Initialize(Func<Task<bool>> before = null, Func<Task> after = null);
		Task Start(Func<Task<bool>> before = null, Func<Task> after = null);
		Task InitializeRecord(Func<Task<bool>> before = null, Func<Task> after = null);
		Task StartRecord(Func<Task<bool>> before = null, Func<Task> after = null);
		Task StopRecord(Func<Task<bool>> before = null, Func<Task> after = null);
		Task UninitializeRecord(Func<Task<bool>> before = null, Func<Task> after = null);
		Task Stop(Func<Task<bool>> before = null, Func<Task> after = null);
		Task Uninitialize(Func<Task<bool>> before = null, Func<Task> after = null);
		Task StartCalibration(Func<Task<bool>> before = null, Func<Task> after = null);
		Task<CalibrationData> StopCalibration(Func<Task<bool>> before = null, Func<Task> after = null);
		Task<Exception> ProcessCalibrationData(bool isAccepted, CalibrationData calibrationData, Func<Task<bool>> before = null, Func<Task> after = null);
	}
}