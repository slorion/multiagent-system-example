using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class PhotoAgentEventJournal
		: EventJournal<PhotoAgentEventJournalHeader, PhotoAgentEventJournalEntry, PhotoAgentEventJournalFooter>
	{

	}
}