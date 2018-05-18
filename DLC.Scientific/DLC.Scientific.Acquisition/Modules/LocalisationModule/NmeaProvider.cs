using DLC.Framework.IO.Ports;
using DLC.Framework.Runtime;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding.Gps;
using DLC.Scientific.Core.Geocoding.Gps.Nmea;
using System;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.LocalisationModule
{
	public class NmeaProvider
		: LocalisationProvider
	{
		public NmeaProvider()
			: base()
		{
			this.Frequency = 50;
		}

		public string SerialPortName { get; set; }
		public BaudRate SerialBaudRate { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (string.IsNullOrEmpty(this.SerialPortName)) throw new InvalidOperationException("SerialPortName is mandatory.");
		}

		protected override Task<IObservable<LocalisationData>> InitializeCore()
		{
			return Task.Run(
				() =>
				{
					IObservable<GeoData> geoDataObservable = ToGeoData(ToNmeaSentence(SerialConnection.CreateCharListener(this.SerialPortName, this.SerialBaudRate, Parity.None, 8, StopBits.One)));

					return (
						from geoData in geoDataObservable
						select new LocalisationData {
							RawData = geoData,
							GpsStatus = (geoData.PositionData.NbSatellites < 4 || geoData.PositionData.Quality == FixType.None || geoData.PrecisionData.Hdop > 4 ? GpsStatus.SignalLost : GpsStatus.Reliable)
						}).Publish().RefCount();
				});
		}

		private static IObservable<string> ToNmeaSentence(IObservable<char> stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");

			return Observable.Create<string>(
				observer =>
				{
					var sb = new StringBuilder();

					return stream.Subscribe(
						ch =>
						{
							if (ch == '$' && sb.Length > 0)
							{
								observer.OnNext(sb.ToString());
								sb.Clear();
							}
							sb.Append(ch);
						});
				});
		}

		private static IObservable<GeoData> ToGeoData(IObservable<string> sentences)
		{
			if (sentences == null) throw new ArgumentNullException("sentences");

			return Observable.Create<GeoData>(
				observer =>
				{
					GeoData data = new GeoData();

					int parseErrorCount = 0;

					return sentences.Subscribe(
						s =>
						{
							NmeaRawData nmeaRawData;

							if (!NmeaRawData.TryParse(s, out nmeaRawData))
							{
								parseErrorCount++;
								if (parseErrorCount <= 5)
									return;
								else
									throw new InvalidOperationException("Invalid sentence (checksum failed).");
							}

							nmeaRawData.FillGeoData(data);

							if (nmeaRawData.TypeCode == TypeCodes.GGA)
							{
								observer.OnNext(data);
								data = CloneHelper.DeepClone(data);
							}
						});
				});
		}
	}
}