using NLog.Fluent;

namespace DLC.Multiagent.Logging
{
	public static class LoggerExtensions
	{
		public static LogBuilder WithAgent(this LogBuilder builder, IAgent agent)
		{
			string agentId;

			if (agent == null)
				agentId = "(null)";
			else
			{
				try
				{
					agentId = agent.Id;
				}
				catch
				{
					agentId = "(error)";
				}
			}

			return WithAgent(builder, agentId);
		}

		public static LogBuilder WithAgent(this LogBuilder builder, string agentId)
		{
			return builder.Property("agent-id", agentId);
		}
	}
}