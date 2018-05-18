using System;

namespace DLC.Scientific.Core.Journalisation
{
	[Serializable]
	public class JournalEntry
	{
		public DateTime DateTime { get; set; }
		public string Comment { get; set; }
	}
}