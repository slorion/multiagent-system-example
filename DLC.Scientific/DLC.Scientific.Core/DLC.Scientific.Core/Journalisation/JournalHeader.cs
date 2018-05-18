using System;

namespace DLC.Scientific.Core.Journalisation
{
	[Serializable]
	public class JournalHeader
	{
		public string Version { get; set; } = "2.16";

		public string Id { get; set; }
		public string Sequenceur { get; set; }

		public string CreationSource { get; set; }
		public DateTime CreationDateTime { get; set; }
	}
}