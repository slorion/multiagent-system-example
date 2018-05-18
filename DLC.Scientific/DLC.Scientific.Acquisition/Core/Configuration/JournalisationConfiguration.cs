using DLC.Scientific.Core.Configuration;

namespace DLC.Scientific.Acquisition.Core.Configuration
{
	public class JournalisationConfiguration
		: BaseConfiguration
	{
		public string RootPath { get; set; }
		public int LogGap { get; set; }
		public double DeviceDistanceFromStartTriggerPoint { get; set; }
		public double DeviceDistanceFromStopTriggerPoint { get; set; }
		public double AcquisitionOffSet { get; set; }
		public string AgentFolderPath { get; set; }
		public string CharacterizationFileName { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (string.IsNullOrEmpty(this.RootPath)) MissingProperty("RootPath");
			if (this.LogGap <= 0) OutOfRangeMin("LogGap", 1);
			if (this.DeviceDistanceFromStartTriggerPoint < 0) OutOfRangeMin("DeviceDistanceFromStartTriggerPoint", 0);
			if (this.DeviceDistanceFromStopTriggerPoint < 0) OutOfRangeMin("DeviceDistanceFromStopTriggerPoint", 0);
			if (string.IsNullOrEmpty(this.AgentFolderPath)) MissingProperty("AgentFolderPath");
		}
	}
}