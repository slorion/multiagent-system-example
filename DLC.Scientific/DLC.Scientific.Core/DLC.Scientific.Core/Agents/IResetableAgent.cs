using DLC.Multiagent;
using System;
using System.ServiceModel;

namespace DLC.Scientific.Core.Agents
{
	[ServiceContract]
	public interface IResetableAgent
		: IAgent
	{
		/// <summary>
		/// Try to put acquisition system in its initial state, not matter its current state.
		/// </summary>
		/// <returns>true if reset was successfull; otherwise, false.</returns>
		[OperationContract]
		bool ResetModule();
	}
}