using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class NavigationAgentEventJournalEntry
		: EventJournalEntry
	{
		public ulong FrameId { get; set; }
		public double? Deviation { get; set; }
		public int State { get; set; }
		public double AbsoluteDistance { get; set; }
		public double? LaneWidthInMeter { get; set; }
		public double? DeviationInCm { get; set; }
	}
}