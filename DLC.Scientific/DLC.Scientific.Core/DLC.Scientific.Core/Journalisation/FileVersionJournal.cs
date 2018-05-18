namespace DLC.Scientific.Core.Journalisation
{
	public class FileVersionJournal
		: Journal<FileVersionJournalHeader, FileVersionJournalEntry, FileVersionJournalFooter>
	{
		public static readonly new string DefaultExtension = ".fvx";
	}
}