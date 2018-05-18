using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class AcquisitionManagerAgentEventJournalFooter
		: EventJournalFooter
	{
		public string Comment { get; set; }
		public bool Validation { get; set; }
	}
}