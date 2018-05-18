using DLC.Scientific.Acquisition.Core.Configuration;
using System;

namespace DLC.Scientific.Acquisition.Agents.SpeedAgent.Configuration
{
	public class SpeedAgentConfiguration
		: AcquisitionAgentConfiguration
	{
		public DistanceAcquisitionMode AcquisitionMode { get; set; }
		public int AcquisitionTimeSavingIntervalInMs { get; set; }

		public override void Validate()
		{
			base.Validate();

			switch (this.AcquisitionMode)
			{
				case DistanceAcquisitionMode.Distance:
					if (this.Journalisation.LogGap < 1) OutOfRangeMin("Journalisation.LogGap", 1);
					break;
				case DistanceAcquisitionMode.Time:
					if (this.AcquisitionTimeSavingIntervalInMs < 1) OutOfRangeMin("AcquisitionTimeSavingIntervalInMs", 1);
					break;
			}
		}
	}
}