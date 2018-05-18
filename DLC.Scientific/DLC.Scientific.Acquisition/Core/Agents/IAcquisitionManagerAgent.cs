using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	[ServiceKnownType(typeof(Rtssc))]
	public interface IAcquisitionManagerAgent
		: IProviderAgent<ProviderData>, IAcquisitionableAgent
	{
		IObservable<AcquisitionManagerStateChangedResult> AcquisitionManagerStateDataSource { get; }

		int MinimumSpeed { [OperationContract] get; }
		int MaximumSpeed { [OperationContract] get; }
		string SelectedVehicle { [OperationContract] get; }

		[OperationContract]
		Task<AcquisitionActionResult> PrepareRecord(string driverFullName, string operatorFullName, string vehicleFullName, string sequenceType);

		[OperationContract]
		Task<AcquisitionActionResult> EngageStartRecord(AcquisitionTriggerMode triggerMode, DirectionBgr? direction, IRtssc rtssc, int? proximityRange);

		[OperationContract]
		Task<AcquisitionActionResult> EngageStopRecord(AcquisitionTriggerMode triggerMode, DirectionBgr? direction, IRtssc rtssc, int? proximityRange, double? distance);

		[OperationContract]
		bool Disengage(bool isStartMode);

		[OperationContract]
		Task<bool> ValidateRecord(bool success, string comment);
	}
}