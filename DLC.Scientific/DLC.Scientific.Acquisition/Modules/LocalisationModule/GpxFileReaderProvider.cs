using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.LocalisationModule
{
	public class GpxFileReaderProvider
		: LocalisationProvider
	{
		public string FilePath { get; set; }
		public bool AutoRepeatTrace { get; set; }
		public int InitialDelayInMs { get; set; }
		public int ReaderFrequencyInMs { get; set; }

		public long DataSourceCount { get; private set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (!File.Exists(this.FilePath)) throw new FileNotFoundException("The file '{0}' cannot be found. Check FilePath in the configuration.", this.FilePath);
			if (this.ReaderFrequencyInMs < 0) throw new InvalidOperationException("ReaderFrequencyInMs must be greater than or equal to 0.");
			if (this.InitialDelayInMs < 0) throw new InvalidOperationException("InitialDelayInMs must be greater than or equal to 0.");
		}

		protected override Task<IObservable<Core.AcquisitionProviders.Model.LocalisationData>> InitializeCore()
		{
			this.Frequency = this.ReaderFrequencyInMs <= 0 ? 50 : 1000 / this.ReaderFrequencyInMs;

			return Task.Run(
				() =>
				{
					// the raw value is used rather than the corrected value
					// because the Localisation provider with recalculate the correction as if it was receiving live values
					var gpxDataSource = GpxReader.LoadGpxData(this.FilePath, false)
						.Select(
							gpxData =>
							{
								var currentGeoData = new GeoData { PositionData = gpxData.PositionData, PrecisionData = gpxData.PrecisionData, VelocityData = gpxData.VelocityData };

								GpsStatus gpsStatus;
								if (gpxData.PositionData.InsData == null)
									gpsStatus = (gpxData.PositionData.NbSatellites < 4 || gpxData.PositionData.Quality == FixType.None || gpxData.PrecisionData.Hdop > 4 ? GpsStatus.SignalLost : GpsStatus.Reliable);
								else
									gpsStatus = (gpxData.PositionData.InsData.Status != NavigationStatus.Locked ? GpsStatus.Initializing : gpxData.PositionData.Longitude == 0 && gpxData.PositionData.Latitude == 0 ? GpsStatus.SignalLost : GpsStatus.Reliable);

								return new LocalisationData { RawData = currentGeoData, GpsStatus = gpsStatus };
							});

					// performance could be improved here, but good enough for now
					this.DataSourceCount = gpxDataSource.LongCount();

					IObservable<LocalisationData> obs;

					if (this.ReaderFrequencyInMs == 0)
						obs = gpxDataSource.ToObservable();
					else
					{
						obs = gpxDataSource.ToObservable()
							.Zip(Observable.Interval(TimeSpan.FromMilliseconds(this.ReaderFrequencyInMs)), (loc, ticks) => loc);
					}

					obs = obs.TakeWhile(_ => this.State > ProviderState.Initialized);

					if (this.AutoRepeatTrace)
						obs = obs.Repeat();

					if (this.InitialDelayInMs > 0)
						obs = obs.DelaySubscription(TimeSpan.FromMilliseconds(this.InitialDelayInMs));

					return obs;
				});
		}
	}
}