using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface ISpeedAgent
		: IProviderAgent<SpeedData>, IAcquisitionableAgent
	{
	}
}