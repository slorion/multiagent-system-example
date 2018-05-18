using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class DistanceAgentEventJournal
		: EventJournal<DistanceAgentEventJournalHeader, DistanceAgentEventJournalEntry>
	{
	}
}