using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Globalization;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Gst
		: Sentence
	{
		public Gst(string sentence, string talkerId)
			: base(talkerId, TypeCodes.GST)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			TimeSpan utc;
			double rmsDeviation;
			double semiMajorDeviation;
			double semiMinorDeviation;
			double semiMajorOrientation;
			double latitudeErrorDeviation;
			double longitudeErrorDeviation;
			double altitudeErrorDeviation;

			string[] datas = sentence.Split(new[] { ',', '*' });

			if (datas.Length != 10)
				throw new ArgumentException("Sentence format is invalid.", "sentence");

			TimeSpan.TryParseExact(datas[1], "HHmmss.FFF", CultureInfo.InvariantCulture, out utc);
			double.TryParse(datas[2], NumberStyles.None, CultureInfo.InvariantCulture, out rmsDeviation);
			double.TryParse(datas[3], NumberStyles.None, CultureInfo.InvariantCulture, out semiMajorDeviation);
			double.TryParse(datas[4], NumberStyles.None, CultureInfo.InvariantCulture, out semiMinorDeviation);
			double.TryParse(datas[5], NumberStyles.None, CultureInfo.InvariantCulture, out semiMajorOrientation);
			double.TryParse(datas[6], NumberStyles.None, CultureInfo.InvariantCulture, out latitudeErrorDeviation);
			double.TryParse(datas[7], NumberStyles.None, CultureInfo.InvariantCulture, out longitudeErrorDeviation);
			double.TryParse(datas[8], NumberStyles.None, CultureInfo.InvariantCulture, out altitudeErrorDeviation);

			string checksum = datas[9];

			this.Utc = utc;
			this.RmsDeviation = rmsDeviation;
			this.SemiMajorDeviation = semiMajorDeviation;
			this.SemiMinorDeviation = semiMinorDeviation;
			this.SemiMajorOrientation = semiMajorOrientation;
			this.LatitudeErrorDeviation = latitudeErrorDeviation;
			this.LongitudeErrorDeviation = longitudeErrorDeviation;
			this.AltitudeErrorDeviation = altitudeErrorDeviation;
			this.Checksum = checksum;
		}

		/// <summary>
		/// Gets the altitude error deviation.
		/// </summary>
		public double AltitudeErrorDeviation { get; set; }

		/// <summary>
		/// Gets the checksum.
		/// </summary>
		public string Checksum { get; set; }

		/// <summary>
		/// Gets the latitude error deviation.
		/// </summary>
		public double LatitudeErrorDeviation { get; set; }

		/// <summary>
		/// Gets the longitude error deviation.
		/// </summary>
		public double LongitudeErrorDeviation { get; set; }

		/// <summary>
		/// Gets the RMS deviation.
		/// </summary>
		public double RmsDeviation { get; set; }

		/// <summary>
		/// Gets the semi major deviation.
		/// </summary>
		public double SemiMajorDeviation { get; set; }

		/// <summary>
		/// Gets the semi major orientation.
		/// </summary>
		public double SemiMajorOrientation { get; set; }

		/// <summary>
		/// Gets the semi minor deviation.
		/// </summary>
		public double SemiMinorDeviation { get; set; }

		/// <summary>
		/// Gets the UTC time in hours , minutes, seconds of the GPS position.
		/// </summary>
		public TimeSpan Utc { get; set; }

		public override void FillGeoData(GeoData data)
		{
			data.PositionData.Utc = DateTime.UtcNow.Date.Add(this.Utc);
		}
	}
}
