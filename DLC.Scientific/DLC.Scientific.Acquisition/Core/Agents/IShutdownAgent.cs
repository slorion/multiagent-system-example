using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IShutdownAgent
		: IAcquisitionAgent
	{
		[OperationContract]
		Task ShutdownMachine();

		[OperationContract]
		Task RebootMachine();
	}
}