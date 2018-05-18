using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class DistanceAgentEventJournalEntry
		: EventJournalEntry
	{
		public DateTime AbsoluteTime { get; set; }
		public double AbsoluteElapsedTime { get; set; }
		public double RelativeElapsedTime { get; set; }
		public int Progress { get; set; }
	}
}