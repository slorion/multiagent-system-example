using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class AcquisitionManagerAgentEventJournal
		: EventJournal<AcquisitionManagerAgentEventJournalHeader, AcquisitionManagerAgentEventJournalEntry, AcquisitionManagerAgentEventJournalFooter>
	{
		public void SetFooterSpecificContent(bool validation, string comment)
		{
			this.JournalFooter.Validation = validation;
			this.JournalFooter.Comment = comment;
		}
	}
}