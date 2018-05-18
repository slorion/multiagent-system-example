using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class SpeedAgentEventJournalEntry
		: EventJournalEntry
	{
		public double VitesseGps { get; set; }
		public double VitesseDistance { get; set; }
		public string ActiveMode { get; set; }
		public int Progress { get; set; }
	}
}