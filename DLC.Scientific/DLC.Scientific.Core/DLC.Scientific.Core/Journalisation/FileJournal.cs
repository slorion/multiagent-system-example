namespace DLC.Scientific.Core.Journalisation
{
	public class FileJournal
		: Journal<FileJournalHeader, FileJournalEntry, FileJournalFooter>,
		IFileJournal
	{
		public static readonly new string DefaultExtension = ".fjx";

		FileJournalHeader IFileJournal.FileJournalHeader
		{
			get { return this.JournalHeader; }
		}

		FileJournalFooter IFileJournal.FileJournalFooter
		{
			get { return this.JournalFooter; }
		}
	}
}