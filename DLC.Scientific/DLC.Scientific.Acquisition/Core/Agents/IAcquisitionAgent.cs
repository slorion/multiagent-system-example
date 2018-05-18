using DLC.Scientific.Core.Agents;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IAcquisitionAgent
		: IOperationalAgent
	{
	}
}