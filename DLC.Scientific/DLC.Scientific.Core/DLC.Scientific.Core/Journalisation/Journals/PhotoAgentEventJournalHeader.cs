using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class PhotoAgentEventJournalHeader
		: EventJournalHeader
	{
		public string Name { get; set; }
		public string TypeImage { get; set; }
		public string SaveFolder { get; set; }
		public string CameraSerialNumber { get; set; }
		public DateTime LastCameraPhysicalModificationDate { get; set; }
	}
}