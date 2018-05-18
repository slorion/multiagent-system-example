using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IDistanceAgent
		: IProviderAgent<DistanceData>, IAcquisitionableAgent, ICalibrableAgent
	{
		int ReferenceEncoderNumber { [OperationContract] get; }

		int PPKMLeft { [OperationContract] get; }
		int PPKMRight { [OperationContract] get; }
		int IntervalLength { [OperationContract] get; }
	}
}