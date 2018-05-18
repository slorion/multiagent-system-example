using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Globalization;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Vtg
		: Sentence
	{
		public Vtg(string sentence, string talkerId)
			: base(talkerId, TypeCodes.VTG)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			double courseTrueHeading;
			double courseMagneticHeading;
			double speedKnots;
			double speedKmH;

			string[] datas = sentence.Split(new[] { ',', '*' });

			if (datas.Length < 9)
				throw new ArgumentException("Sentence format is invalid.", "sentence");

			double.TryParse(datas[1], NumberStyles.Number, CultureInfo.InvariantCulture, out courseTrueHeading);
			double.TryParse(datas[3], NumberStyles.Number, CultureInfo.InvariantCulture, out courseMagneticHeading);
			double.TryParse(datas[5], NumberStyles.Number, CultureInfo.InvariantCulture, out speedKnots);
			double.TryParse(datas[7], NumberStyles.Number, CultureInfo.InvariantCulture, out speedKmH);

			string referenceTrueHeading = datas[2];
			string referenceMagneticHeading = datas[4];
			string unitsKnots = datas[6];
			string unitsKmH = datas[8];
			string checksum = datas[9];

			this.CourseTrueHeading = courseTrueHeading;
			this.ReferenceTrueHeading = referenceTrueHeading;
			this.CourseMagneticHeading = courseMagneticHeading;
			this.ReferenceMagneticHeading = referenceMagneticHeading;
			this.SpeedKnots = speedKnots;
			this.UnitsKnots = unitsKnots;
			this.SpeedKmH = speedKmH;
			this.UnitsKmH = unitsKmH;
			this.Checksum = checksum;
		}

		/// <summary>
		/// Gets the checksum.
		/// </summary>
		public string Checksum { get; set; }

		/// <summary>
		/// Gets the course magnetic heading.
		/// </summary>
		public double CourseMagneticHeading { get; set; }

		/// <summary>
		/// Gets the course true heading.
		/// </summary>
		public double CourseTrueHeading { get; set; }

		/// <summary>
		/// Gets the reference magnetic heading.
		/// </summary>
		public string ReferenceMagneticHeading { get; set; }

		/// <summary>
		/// Gets the reference true heading.
		/// </summary>
		public string ReferenceTrueHeading { get; set; }

		/// <summary>
		/// Gets the speed km H.
		/// </summary>
		public double SpeedKmH { get; set; }

		/// <summary>
		/// Gets the speed knots.
		/// </summary>
		public double SpeedKnots { get; set; }

		/// <summary>
		/// Gets the units km H.
		/// </summary>
		public string UnitsKmH { get; set; }

		/// <summary>
		/// Gets the units knots.
		/// </summary>
		public string UnitsKnots { get; set; }

		public override void FillGeoData(GeoData data)
		{
			data.VelocityData.SpeedKmh = this.SpeedKmH;
		}
	}
}
