using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IFileTransferAgent
		: IAcquisitionAgent
	{
		IObservable<FileTransferData> FileTransferDataSource { get; }
		IObservable<bool> IsTransferringDataSource { get; }

		bool IsTransferring { [OperationContract] get; }

		[OperationContract]
		void StartTransferring();

		[OperationContract]
		Task StopTransferring();
	}
}