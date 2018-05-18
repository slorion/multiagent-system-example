using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	[ServiceContract]
	public interface IPeerCommunicationAgent
		: IAgent
	{
		[OperationContract]
		IEnumerable<Tuple<string, AgentDisplayData, IEnumerable<string>>> GetAgentList();

		[OperationContract]
		Task<bool> RecycleAgent(string agentId);

		[OperationContract]
		int EnsureObservableListening(string agentId, string propertyName);

		[OperationContract]
		string GetBrokerLog(bool archive);
	}
}