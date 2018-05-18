using System.ComponentModel;

namespace DLC.Scientific.Core.Journalisation
{
	public interface IJournal
	{
		event AddingNewEventHandler AddingNew;

		JournalHeader EventJournalHeader { get; }
		JournalFooter EventJournalFooter { get; }

		void Add(string comment);
		void Add(JournalEntry entry);
		void Close();
	}
}