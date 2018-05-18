using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface ILocalisationAgent
		: IProviderAgent<LocalisationData>, IAcquisitionableAgent
	{
		double GpsDistanceFromTriggerPoint { [OperationContract] get; }
		int GpsFrequency { [OperationContract] get; }
	}
}