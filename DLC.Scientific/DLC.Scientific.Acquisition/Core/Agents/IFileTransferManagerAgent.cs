using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IFileTransferManagerAgent
		: IAcquisitionAgent
	{
		bool AutoCollapseGrid { [OperationContract] get; }

		IObservable<FileTransferData> FileTransferDataSource { get; }
		IObservable<bool> IsTransferringDataSource { get; }

		bool IsTransferring { [OperationContract] get; }

		[OperationContract]
		Task<ExecutionResult[]> StartTransferring();

		[OperationContract]
		Task<ExecutionResult[]> StopTransferring();
	}
}