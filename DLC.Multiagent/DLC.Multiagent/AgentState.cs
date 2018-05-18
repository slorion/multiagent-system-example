using System;

namespace DLC.Multiagent
{
	public enum AgentState
	{
		Created,
		Idle,
		Activating,
		Activated,
		Deactivating,
		Disposed,
		Failed
	}
}