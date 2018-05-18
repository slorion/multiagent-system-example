using System;

namespace DLC.Multiagent
{
	public enum TryGetAgentResult
	{
		Success,
		NotFound,
		Unreachable,
		ContractNotImplemented,
		NotActivated
	}
}