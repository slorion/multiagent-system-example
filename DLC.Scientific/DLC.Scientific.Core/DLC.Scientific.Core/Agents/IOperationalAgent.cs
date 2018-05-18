using DLC.Multiagent;
using System;
using System.ServiceModel;

namespace DLC.Scientific.Core.Agents
{
	[ServiceContract]
	public interface IOperationalAgent
		: IAgent
	{
		IObservable<Tuple<string, string, OperationalAgentStates>> DependenciesOperationalStateDataSource { get; }

		OperationalAgentStates OperationalState { [OperationContract] get; [OperationContract] set; }
		IObservable<OperationalAgentStates> OperationalStateDataSource { get; }

		[OperationContract]
		bool IsDependencyOperational(string agentTypeName);
	}
}