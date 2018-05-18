using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Collections.Generic;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class SpeedProvider
		: AcquisitionProvider<SpeedData>
	{
		public List<SpeedRanges> SpeedRanges { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.SpeedRanges == null) throw new InvalidOperationException("SpeedRanges was not provided in the configuration.");
		}
	}
}