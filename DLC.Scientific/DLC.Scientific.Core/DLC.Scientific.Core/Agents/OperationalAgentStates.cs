using System;

namespace DLC.Scientific.Core.Agents
{
	/// <summary>
	/// Errors that can make an agent non-operational.
	/// </summary>
	[Flags]
	public enum OperationalAgentStates
	{
		None = 0,

		/// <summary>
		/// Agent is not running.
		/// </summary>
		AgentNotRunning = 1,

		/// <summary>
		/// A problem prevents the agent from running, e.g. a missing or bad configuration.
		/// </summary>
		InternalAgentError = 2,

		/// <summary>
		/// The related module has failed.
		/// </summary>
		ModuleError = 4,

		/// <summary>
		/// An external mandatory agent dependency is missing or non-operational.
		/// </summary>
		ExternalMandatoryAgentMissing = 8
	}
}