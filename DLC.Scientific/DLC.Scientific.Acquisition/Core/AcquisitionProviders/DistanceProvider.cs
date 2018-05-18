using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class DistanceProvider
		: AcquisitionProvider<DistanceData>
	{
		public int IntervalLength { get; set; }
		public int EncoderNumber { get; set; }
		public int PPKMLeft { get; set; }
		public int PPKMRight { get; set; }
		public int NbPulseEncoder { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.IntervalLength < 0) throw new InvalidOperationException("IntervalLength must be greater than or equal to 0.");
			if (this.EncoderNumber < 0 || this.EncoderNumber > 2) throw new InvalidOperationException("EncoderNumber must be either 0 (none), 1 (left) or 2 (right).");
			if (this.PPKMLeft < 0) throw new InvalidOperationException("PPKMLeft must be greater than or equal to 0.");
			if (this.PPKMRight < 0) throw new InvalidOperationException("PPKMRight must be greater than or equal to 0.");
			if (this.NbPulseEncoder < 0) throw new InvalidOperationException("NbPulseEncoder must be greater than or equal to 0.");
		}
	}
}