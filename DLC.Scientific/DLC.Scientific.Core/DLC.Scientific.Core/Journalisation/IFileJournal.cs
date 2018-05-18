namespace DLC.Scientific.Core.Journalisation
{
	public interface IFileJournal
		: IJournal
	{
		FileJournalHeader FileJournalHeader { get; }
		FileJournalFooter FileJournalFooter { get; }
	}
}