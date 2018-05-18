using System;

namespace DLC.Multiagent.Logging
{
	[Serializable]
	public class BrokerLogEntry
	{
		public DateTimeOffset Timestamp { get; set; }
		public BrokerLogLevel Level { get; set; }
		public string Source { get; set; }
		public string AgentId { get; set; }
		public string Message { get; set; }
		public string Exception { get; set; }
	}
}