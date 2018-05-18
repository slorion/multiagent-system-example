using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class PhotoAgentEventJournalEntry
		: EventJournalEntry
	{
		public string ImageName { get; set; }
		public string CameraName { get; set; }
		public int Progress { get; set; }
		public double FrameRate { get; set; }
		public string ExposureMode { get; set; }
		public long ExposureAutoTarget { get; set; }
		public double ExposureTime { get; set; }
		public string IrisMode { get; set; }
		public long IrisAutoTarget { get; set; }
		public long IrisVideoLevel { get; set; }
		public double SaturationScore { get; set; }
		public float PixelAverageValue { get; set; }
		public string TypeCasLimite { get; set; }
		public int FrameId { get; set; }
	}
}