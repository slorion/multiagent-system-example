using DLC.Framework.Extensions;
using DLC.Framework.Reactive;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.SpeedModule
{
	public class CalculatedSpeedProvider
		: SpeedProvider
	{
		public CalculatedSpeedProvider()
		{
			// Be careful not to set this value too low. At low speed, average value calculations might get wrong.
			this.DistanceDataTimeout = TimeSpan.FromMilliseconds(500);

			this.Hysteresis = 5;
			this.Threshold = 10;
			this.DistanceSpeedCountForAverage = 5;
		}

		public IObservable<DistanceData> DistanceDataSource { get; set; }
		public IObservable<LocalisationData> LocalisationDataSource { get; set; }
		public Func<bool> IsDistanceOperationalCallback { get; set; }
		public Func<bool> IsLocalisationOperationalCallback { get; set; }

		public double Hysteresis { get; set; }
		public double Threshold { get; set; }
		public TimeSpan DistanceDataTimeout { get; set; }
		public int DistanceSpeedCountForAverage { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.DistanceDataSource == null && this.LocalisationDataSource == null) throw new InvalidOperationException("At least one data source (distance or localisation) must be provided.");
			if (this.DistanceDataSource != null && this.IsDistanceOperationalCallback == null) throw new InvalidOperationException("If DistanceDataSource is provided, then IsDistanceOperationalCallback must also be set.");
			if (this.LocalisationDataSource != null && this.IsLocalisationOperationalCallback == null) throw new InvalidOperationException("If LocalisationDataSource is provided, then IsLocalisationOperationalCallback must also be set.");

			if (this.Hysteresis < 0) throw new InvalidOperationException("Hysteresis must be greater than or equal to 0.");
			if (this.Threshold < 0) throw new InvalidOperationException("Threshold must be greater than or equal to 0.");
			if (this.DistanceSpeedCountForAverage < 1) throw new InvalidOperationException("DistanceSpeedCountForAverage must be greater than 0.");
		}

		protected override Task<IObservable<SpeedData>> InitializeCore()
		{
			if (this.DistanceDataSource == null)
			{
				this.DistanceDataSource = new SubjectSlim<DistanceData>();
				this.IsDistanceOperationalCallback = () => false;
			}
			else
			{
				// we must be able to keep sending data even if distanceDataSource completes
				// so we merge it with a subject that never completes
				this.DistanceDataSource = this.DistanceDataSource.Merge(new SubjectSlim<DistanceData>());
			}

			if (this.LocalisationDataSource == null)
			{
				this.LocalisationDataSource = new SubjectSlim<LocalisationData>();
				this.IsLocalisationOperationalCallback = () => false;
			}

			return Task.Run(
				() =>
				{
					var speedBuffer = new ConcurrentQueue<double>();

					DistanceData lastDistance = null;
					SpeedData lastSpeed = null;
					double? lastDistanceSpeed = null;
					double? lastGpsSpeed = null;

					var distanceDataSource = this.DistanceDataSource
						// filter invalid data
						.Where(
							data =>
							{
								if (lastDistance == null)
								{
									// no previous data
									// so save first data point
									// but speed cannot be calculated yet

									lastDistance = data;
									return false;
								}

								var deltaDistance = data.AbsoluteDistance - lastDistance.AbsoluteDistance;
								var deltaTime = data.Timestamp - lastDistance.Timestamp;

								// case when distance was reset on acquisition start
								if (deltaDistance < 0)
									lastDistance = data;

								return deltaTime.TotalMilliseconds > 0 && deltaDistance >= 0;
							})
						// odometric speed calculation
						.Select(
							data =>
							{
								Debug.Assert(lastDistance != null);

								var deltaDistance = data.AbsoluteDistance - lastDistance.AbsoluteDistance;
								var deltaTime = data.Timestamp - lastDistance.Timestamp;

								speedBuffer.Enqueue(deltaDistance * (3600 / deltaTime.TotalMilliseconds));

								double tmpSpeed;
								if (speedBuffer.Count > this.DistanceSpeedCountForAverage)
									speedBuffer.TryDequeue(out tmpSpeed);

								lastDistance = data;
								return new SpeedData { CurrentSpeed = speedBuffer.Average(), CurrentDistance = lastDistance.AbsoluteDistance, SpeedSource = SpeedActiveMode.Distance };
							})
						// return odometric speed immediately or wait until timeout is reached
						.Buffer(this.DistanceDataTimeout, 1)
						// if timeout has been reached and odometer is operational, assume that speed is equal to 0
						.Select(
							buffer =>
							{
								if (buffer.Count > 0)
									return buffer[0];
								else if (this.IsDistanceOperationalCallback())
								{
									speedBuffer.Enqueue(0);

									double tmpSpeed;
									if (speedBuffer.Count > this.DistanceSpeedCountForAverage)
										speedBuffer.TryDequeue(out tmpSpeed);

									return new SpeedData { CurrentSpeed = speedBuffer.Average(), SpeedSource = SpeedActiveMode.Distance };
								}
								else
									return null;
							})
						// filter null values in case odometer is not operational
						.Where(data => data != null)
						// save last odometric speed
						.Select(
							data =>
							{
								lastDistanceSpeed = data.CurrentSpeed;
								return data;
							})
						.Publish().RefCount();

					var gpsDataSource = this.LocalisationDataSource
						// filter invalid GPS data and data received while GPS is not operational
						.Where(data => data.GpsStatus != GpsStatus.SignalLost && data.CorrectedData.VelocityData.SpeedKmh >= 0 && this.IsLocalisationOperationalCallback())
						// save last GPS speed
						.Select(
							data =>
							{
								var gpsSpeed = new SpeedData { CurrentSpeed = data.CorrectedData.VelocityData.SpeedKmh, SpeedSource = SpeedActiveMode.Gps };
								lastGpsSpeed = gpsSpeed.CurrentSpeed;
								return gpsSpeed;
							})
						.Publish().RefCount();

					return distanceDataSource.Merge(gpsDataSource)
						// return current speed based on data sources state and hysteresis
						.Select(
							data =>
							{
								SpeedActiveMode source;
								if (data.CurrentSpeed < 0)
									source = SpeedActiveMode.None;
								else if (data.CurrentSpeed < (this.Threshold - this.Hysteresis))
									source = SpeedActiveMode.Distance;
								else if (data.CurrentSpeed > (this.Threshold + this.Hysteresis))
									source = SpeedActiveMode.Gps;
								else
									source = lastSpeed == null ? SpeedActiveMode.Gps : lastSpeed.SpeedSource;

								data.DistanceSpeed = data.SpeedSource == SpeedActiveMode.Distance ? data.CurrentSpeed : this.IsDistanceOperationalCallback() ? lastDistanceSpeed : null;
								data.GpsSpeed = data.SpeedSource == SpeedActiveMode.Gps ? data.CurrentSpeed : this.IsLocalisationOperationalCallback() ? lastGpsSpeed : null;
								Debug.Assert(data.DistanceSpeed != null || data.GpsSpeed != null);

								if (source == SpeedActiveMode.None)
								{
									data.CurrentSpeed = -1;
									data.SpeedSource = SpeedActiveMode.None;
								}
								else if (source == SpeedActiveMode.Distance && data.DistanceSpeed != null)
								{
									data.CurrentSpeed = data.DistanceSpeed.Value;
									data.SpeedSource = SpeedActiveMode.Distance;
								}
								else if (source == SpeedActiveMode.Gps && data.GpsSpeed != null)
								{
									data.CurrentSpeed = data.GpsSpeed.Value;
									data.SpeedSource = SpeedActiveMode.Gps;
								}

								data.IsInRange = this.SpeedRanges.Any(ranges => Math.Floor(data.CurrentSpeed).Between(ranges.MinSpeed, ranges.MaxSpeed, inclusive: true));

								lastSpeed = data;

								return data;
							})
						.Publish().RefCount();
				});
		}
	}
}