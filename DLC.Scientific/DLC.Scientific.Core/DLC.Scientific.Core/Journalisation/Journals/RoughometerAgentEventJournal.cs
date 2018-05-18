using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class RoughometerAgentEventJournal
		: EventJournal<EventJournalHeader, EventJournalEntry, EventJournalFooter>
	{
	}
}