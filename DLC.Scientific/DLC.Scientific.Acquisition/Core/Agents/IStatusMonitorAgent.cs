using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IStatusMonitorAgent
		: IAcquisitionAgent
	{
		IObservable<Tuple<SerializableAgentInformation, ProviderState>> AgentsProviderStateDataSource { [OperationContract]  get; }

		[OperationContract]
		Task QueryStateForAllAgents();
	}
}