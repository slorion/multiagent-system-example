using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class EventPanelAgentEventJournalEntry
		: EventJournalEntry
	{
		public int Id { get; set; }
		public string EventType { get; set; }
		public string EventState { get; set; }
		public bool IsSnapShot { get; set; }
		public double ManualCorrection { get; set; }
		public double Progress { get; set; }
		public int Severity { get; set; }
		
		public double? AbsoluteDistance { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public double? Altitude { get; set; }
		public DateTime? GpsTime { get; set; }

		public override string ToString()
		{
			return string.Format("{0} -> {1} - {2}/100 - {3}mètre(s) - {4} {5}",
				this.EventType,
				this.EventState,
				this.Severity,
				this.Progress,
				this.ManualCorrection == 0 ? "" : string.Format("Correction : {0} mètre(s).", this.ManualCorrection),
				string.IsNullOrEmpty(this.Comment) ? "" : string.Format("{0}.", this.Comment));
		}
	}
}