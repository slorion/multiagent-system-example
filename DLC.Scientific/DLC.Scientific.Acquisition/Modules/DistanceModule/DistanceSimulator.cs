using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.DistanceModule
{
	public class DistanceSimulator
		: DistanceProvider
	{
		protected const int PulsePerMeter = 260;
		protected int _absoluteDistance = 0;

		public int SimulatorFrequencyInMetersPerSecond { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.SimulatorFrequencyInMetersPerSecond < 1)
				throw new InvalidOperationException("SimulatorFrequencyInMetersPerSecond must be greater than 0.");
		}

		protected override Task<IObservable<DistanceData>> InitializeCore()
		{
			return Task.FromResult(
				Observable.Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(1000 / this.SimulatorFrequencyInMetersPerSecond))
					.Select(ticks => new DistanceData { AbsoluteDistance = _absoluteDistance++, ReferenceEncoderNumber = 1, AbsoluteLeftPulseCount = _absoluteDistance * PulsePerMeter, AbsoluteRightPulseCount = _absoluteDistance * PulsePerMeter }));
		}

		protected override Task StartRecordCore()
		{
			_absoluteDistance = 0;
			return base.StartRecordCore();
		}

		public override bool CanCalibrate { get { return true; } }

		protected override Task StartCalibrationCore()
		{
			_absoluteDistance = 0;
			return Task.FromResult(0);
		}
		protected override Task<CalibrationData> StopCalibrationCore()
		{
			return Task.FromResult((CalibrationData) new DistanceCalibrationData { ReferenceEncoderNumber = this.EncoderNumber, IntervalLength = this.IntervalLength, PpkmLeft = 270000, PpkmRight = 275000 });
		}

		protected override Task<Exception> ProcessCalibrationDataCore(bool isAccepted, CalibrationData calibrationData)
		{
			return base.ProcessCalibrationDataCore(isAccepted, calibrationData);
		}
	}
}