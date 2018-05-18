using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	public class FileProcessingJournal
		: Journal<FileHeader, FileProcessingJournalEntry, FileFooter>
	{
		public static readonly new string DefaultExtension = ".fpx";
	}
}