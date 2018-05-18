using System;
using System.ServiceModel;

namespace DLC.Multiagent
{
	[ServiceContract]
	public interface IVisibleAgent
		: IAgent
	{
		bool AutoShowUI { [OperationContract] get; }
		bool ShowErrorListOnLoad { [OperationContract] get; }
		string MainUITypeName { [OperationContract] get; }
		string MainUIAgentTypeName { [OperationContract] get; }
	}
}