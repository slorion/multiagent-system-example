using System;

namespace DLC.Scientific.Core.Journalisation
{
	internal interface IJournalInternal
	{
		Type JournalHeaderType { get; }
		Type JournalEntryType { get; }
		Type JournalFooterType { get; }

		JournalHeader JournalHeader { set; }
		JournalFooter JournalFooter { set; }
	}
}