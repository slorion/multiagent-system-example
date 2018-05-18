using DLC.Scientific.Acquisition.Core.Configuration;
using System;

namespace DLC.Scientific.Acquisition.Agents.LocalisationAgent.Configuration
{
	public class LocalisationAgentConfiguration 
		: AcquisitionAgentConfiguration
	{
		public double OffsetFromTriggerPoint { get; set; }
		public bool UseOnlyTrustworthyData { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (this.OffsetFromTriggerPoint < 0) OutOfRangeMin("OffsetFromTriggerPoint", 0);
		}
	}
}