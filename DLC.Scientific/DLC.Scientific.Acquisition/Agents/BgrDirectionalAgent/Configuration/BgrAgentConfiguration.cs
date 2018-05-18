using DLC.Scientific.Acquisition.Core.Configuration;
using System;

namespace DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.Configuration
{
	public class BgrAgentConfiguration
		: AcquisitionAgentConfiguration
	{
		public int AutoCorrectDelta { get; set; }
		public int ManualSearchRadiusInMeters { get; set; }
		public int AutoSearchRadiusInMeters { get; set; }
		public int AutoSearchIntervalInMs { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (this.AutoCorrectDelta < 0) OutOfRangeMin("AutoCorrectDelta", 0);
			if (this.ManualSearchRadiusInMeters < 0) OutOfRangeMin("ManualSearchRadiusInMeters", 0);
			if (this.AutoSearchRadiusInMeters < 0) OutOfRangeMin("AutoSearchRadiusInMeters", 0);
			if (this.AutoSearchIntervalInMs < 0) OutOfRangeMin("AutoSearchIntervalInMs", 0);
		}
	}
}