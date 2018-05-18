using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class DistanceAgentEventJournalHeader
		: EventJournalHeader
	{
		public string ActiveMode { get; set; }
		public int Ppkm { get; set; }
	}
}