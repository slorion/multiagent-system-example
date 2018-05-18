using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class AudiometerAgentEventJournal
		: EventJournal<EventJournalHeader, EventJournalEntry, EventJournalFooter>
	{
	}
}