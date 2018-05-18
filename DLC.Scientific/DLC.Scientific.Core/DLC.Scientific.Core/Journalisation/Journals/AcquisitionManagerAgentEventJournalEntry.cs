using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class AcquisitionManagerAgentEventJournalEntry
		: EventJournalEntry
	{
		public string Action { get; set; }
		public string RTSSCDebut { get; set; }
		public string RTSSCFin { get; set; }
		public string Distance { get; set; }
		public int Progress { get; set; }
	}
}