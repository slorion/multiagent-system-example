using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class SpeedAgentEventJournal
		: EventJournal<SpeedAgentEventJournalHeader, SpeedAgentEventJournalEntry>
	{
	}
}