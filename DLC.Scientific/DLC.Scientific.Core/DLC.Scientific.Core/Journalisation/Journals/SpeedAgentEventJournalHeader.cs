using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class SpeedAgentEventJournalHeader 
		: EventJournalHeader
	{
		public double BorneSuperieure { get; set; }
		public double BorneInferieure { get; set; }
		public double Tolerance { get; set; }
	}
}