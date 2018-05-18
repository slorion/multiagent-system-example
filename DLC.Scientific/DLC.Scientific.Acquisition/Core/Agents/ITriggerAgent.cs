using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface ITriggerAgent
		: IProviderAgent<TriggerData>
	{
		Task WaitForTrigger(TriggerMode mode);
		Task CancelWaitForTrigger(TriggerMode mode);
	}
}