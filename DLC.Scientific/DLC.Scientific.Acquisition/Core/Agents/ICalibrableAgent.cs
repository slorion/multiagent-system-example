using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface ICalibrableAgent
		: IAcquisitionAgent
	{
		[OperationContract]
		Task StartCalibration();

		[OperationContract]
		Task<CalibrationData> StopCalibration();

		[OperationContract]
		Task<Exception> ProcessCalibrationData(bool isAccepted, CalibrationData calibrationData);

		IObservable<CalibrationData> CalibrationDataSource { get; }
	}
}