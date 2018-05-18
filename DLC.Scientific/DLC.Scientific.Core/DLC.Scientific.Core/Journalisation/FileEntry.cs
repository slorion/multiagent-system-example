namespace DLC.Scientific.Core.Journalisation
{
	public class FileEntry
		: JournalEntry
	{
		public string FileName { get; set; }
		public string RelativePath { get; set; }
		public string AbsolutePath { get; set; }
		public string Checksum { get; set; }
	}
}