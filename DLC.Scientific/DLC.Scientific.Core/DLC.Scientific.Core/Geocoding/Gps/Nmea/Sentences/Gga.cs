using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Globalization;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Gga
		: Sentence
	{
		public Gga(string sentence, string talkerId)
			: base(talkerId, TypeCodes.GGA)
		{
			if (String.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			double antennaAltitude = 0;
			double geoidalSep = 0;
			double hdop = 0;
			int nbSatellites = 0;
			int qualityIndicator = 0;
			TimeSpan utc = TimeSpan.Zero;
			int diffAge = 0;

			double nmeaLatitude = 0;
			double nmeaLongitude = 0;

			string[] datas = sentence.Split(',');

			if (datas.Length != 15)
				throw new ArgumentException("Sentence format is invalid.", "sentence");

			if (!TimeSpan.TryParseExact(datas[1], @"hhmmss\.ff", CultureInfo.InvariantCulture, out utc))
				TimeSpan.TryParseExact(datas[1], @"hhmmss\.fff", CultureInfo.InvariantCulture, out utc); //Pour le simulateur

			double.TryParse(datas[2], NumberStyles.Number, CultureInfo.InvariantCulture, out nmeaLatitude);
			double.TryParse(datas[4], NumberStyles.Number, CultureInfo.InvariantCulture, out nmeaLongitude);
			int.TryParse(datas[6], out qualityIndicator);
			int.TryParse(datas[7], out nbSatellites);
			double.TryParse(datas[8], NumberStyles.Number, CultureInfo.InvariantCulture, out hdop);
			double.TryParse(datas[9], NumberStyles.Number, CultureInfo.InvariantCulture, out antennaAltitude);
			double.TryParse(datas[11], NumberStyles.Number, CultureInfo.InvariantCulture, out geoidalSep);

			// For an unknown reason NMEA specs states that the differential age should
			// have a number in it, but it's received with null
			int.TryParse(datas[13], out diffAge);

			string nmeaLatitudeDirection = datas[3];
			string nmeaLongitudeDirection = datas[5];

			string altitudeUnit = datas[10];
			string geoidalUnit = datas[12];

			string refStation = datas[14].Split('*')[0];
			string checksum = "*" + datas[14].Split('*')[1].Split('\\')[0];

			this.DiffAge = diffAge;
			this.AltitudeUnit = altitudeUnit;
			this.AntennaAltitude = antennaAltitude;
			this.Checksum = checksum;
			this.GeoidalSeparation = geoidalSep;
			this.GeoidalUnit = geoidalUnit;
			this.Hdop = hdop;
			this.Latitude = nmeaLatitude;
			this.LatitudeDirection = nmeaLatitudeDirection;
			this.Longitude = nmeaLongitude;
			this.LongitudeDirection = nmeaLongitudeDirection;
			this.NbSatellites = nbSatellites;
			this.QualityIndicator = qualityIndicator;
			this.RefStation = refStation;
			this.Utc = utc;
		}

		/// <summary>
		/// Gets the altitude unit. M = meters
		/// </summary>
		public string AltitudeUnit { get; set; }

		/// <summary>
		/// Gets the antenna altitude.
		/// </summary>
		public double AntennaAltitude { get; set; }

		/// <summary>
		/// Gets the checksum.
		/// </summary>
		public string Checksum { get; set; }

		/// <summary>
		/// Gets the differential age of corrections in seconds
		/// </summary>
		public int DiffAge { get; set; }

		/// <summary>
		/// Gets the geoidal separation. ( needs geoidal height option )
		/// </summary>
		public double GeoidalSeparation { get; set; }

		/// <summary>
		/// Gets the geoidal separation unit. M = meters
		/// </summary>
		public string GeoidalUnit { get; set; }

		/// <summary>
		/// Gets the HDOP. 0.0 to 9.9
		/// </summary>
		public double Hdop { get; set; }

		/// <summary>
		/// Gets the degree/minutes latitude.
		/// </summary>
		public double Latitude { get; set; }

		/// <summary>
		/// Gets the latitude direction from equator.
		/// </summary>
		public string LatitudeDirection { get; set; }

		/// <summary>
		/// Gets the degree/minutes longitude.
		/// </summary>
		public double Longitude { get; set; }

		/// <summary>
		/// Gets the longitude direction from prime meridian.
		/// </summary>
		public string LongitudeDirection { get; set; }

		/// <summary>
		/// Gets the number of satellites used in position computation.
		/// </summary>
		public int NbSatellites { get; set; }

		/// <summary>
		/// Gets the quality indicator.
		/// 0 = no position, 1 = undifferentially correct position,
		/// 2 = differentially correct position, 9 = position computed using almanac
		/// </summary>
		public int QualityIndicator { get; set; }

		/// <summary>
		/// Gets the Reference station identification.
		/// </summary>
		public string RefStation { get; set; }

		/// <summary>
		/// Gets the UTC time in hours , minutes, seconds of the GPS position.
		/// </summary>
		public TimeSpan Utc { get; set; }

		public override void FillGeoData(GeoData data)
		{
			// In case of GGA message, we have the time of day but date isn't received.
			// We can assume that it's the UTC time of the UTC date.
			data.PositionData.Utc = DateTime.UtcNow.Date.Add(this.Utc);
			data.PositionData.Latitude = ConvertMinDecToDecimalDegree(this.Latitude, this.LatitudeDirection);
			data.PositionData.Longitude = ConvertMinDecToDecimalDegree(this.Longitude, this.LongitudeDirection);
			data.PositionData.Altitude = this.AntennaAltitude;
			data.PositionData.NbSatellites = this.NbSatellites;
			data.PositionData.DifferentialDataAge = this.DiffAge;
			data.PositionData.DifferentialStationId = this.RefStation;
			data.PositionData.GeoIdHeight = this.GeoidalSeparation;

			switch (this.QualityIndicator)
			{
				case 1:
					data.PositionData.Quality = FixType.Fix;
					break;
				case 2:
					data.PositionData.Quality = FixType.Diff;
					break;
				case 4:
					data.PositionData.Quality = FixType.RTKfixed;
					break;
				case 5:
					data.PositionData.Quality = FixType.RTKfloating;
					break;
				case 9:
					data.PositionData.Quality = FixType.WAAS;
					break;
				default:
					data.PositionData.Quality = FixType.None;
					break;
			}
		}
	}
}
