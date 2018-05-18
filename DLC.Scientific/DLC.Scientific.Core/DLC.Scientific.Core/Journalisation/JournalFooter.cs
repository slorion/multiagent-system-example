using System;

namespace DLC.Scientific.Core.Journalisation
{
	[Serializable]
	public class JournalFooter
	{
		public DateTime CloseDateTime { get; set; }
		public int NumberOfEntry { get; set; }
		public bool Repaired { get; set; }
	}
}