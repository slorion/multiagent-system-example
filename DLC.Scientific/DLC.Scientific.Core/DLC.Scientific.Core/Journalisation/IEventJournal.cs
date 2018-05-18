namespace DLC.Scientific.Core.Journalisation
{
	public interface IEventJournal
		: IJournal
	{
		EventJournalHeader EventJournalHeader { get; }
		EventJournalFooter EventJournalFooter { get; }
	}
}