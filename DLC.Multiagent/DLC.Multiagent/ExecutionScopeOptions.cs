using System;

namespace DLC.Multiagent
{
	[Flags]
	public enum ExecutionScopeOptions
	{
		Local = 1,
		Remote = 2,
		All = Local | Remote
	}
}