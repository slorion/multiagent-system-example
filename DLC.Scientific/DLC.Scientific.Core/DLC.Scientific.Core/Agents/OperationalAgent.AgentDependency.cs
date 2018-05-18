namespace DLC.Scientific.Core.Agents
{
	partial class OperationalAgent<TAgentConfiguration, TModuleConfiguration>
	{
		private class AgentDependency
		{
			public AgentDependency(bool isMandatory)
			{
				this.IsMandatory = isMandatory;
				this.State = OperationalAgentStates.AgentNotRunning;
			}

			public bool IsMandatory { get; private set; }
			public OperationalAgentStates State { get; set; }
		}
	}
}