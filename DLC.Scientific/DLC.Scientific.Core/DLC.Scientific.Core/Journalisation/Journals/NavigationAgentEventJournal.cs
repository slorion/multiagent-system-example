﻿using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class NavigationAgentEventJournal
		: EventJournal<NavigationAgentEventJournalHeader, NavigationAgentEventJournalEntry, NavigationAgentEventJournalFooter>
	{

	}
}