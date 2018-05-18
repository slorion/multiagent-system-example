using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IProviderAgent
		: IAcquisitionAgent
	{
		long DataReceivedCount { [OperationContract] get; }

		ProviderState ProviderState { [OperationContract] get; }
		IObservable<ProviderState> ProviderStateDataSource { [OperationContract] get; }

		string SystemVersion { [OperationContract] get; }

		[OperationContract]
		Task<AcquisitionActionResult> Initialize(InitializeAcquisitionParameter parameters);

		[OperationContract]
		Task<AcquisitionActionResult> Start(StartAcquisitionParameter parameters);

		[OperationContract]
		Task<AcquisitionActionResult> Stop(StopAcquisitionParameter parameters);

		[OperationContract]
		Task<AcquisitionActionResult> Uninitialize(UninitializeAcquisitionParameter parameters);
	}

	[ServiceContract]
	public interface IProviderAgent<out TData>
		: IProviderAgent
		where TData : ProviderData
	{
		TData CurrentData { [OperationContract] get; }
		IObservable<TData> DataSource { get; }
	}
}