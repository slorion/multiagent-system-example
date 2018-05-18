using DLC.Multiagent.Configuration;
using DLC.Multiagent.Logging;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	[ServiceContract]
	public interface IAgent
		: IDisposable
	{
		AgentState State { [OperationContract] get; }
		IObservable<AgentState> StateDataSource { get; }

		string Id { [OperationContract] get; }
		string ConfigurationFilePath { [OperationContract] get; }
		AgentDisplayData DisplayData { [OperationContract] get; }

		[OperationContract]
		Task<bool> Activate();

		[OperationContract]
		Task<bool> Deactivate();

		[OperationContract]
		bool Ping();

		void LoadConfiguration(string agentId, AgentConfiguration configuration);
	}
}