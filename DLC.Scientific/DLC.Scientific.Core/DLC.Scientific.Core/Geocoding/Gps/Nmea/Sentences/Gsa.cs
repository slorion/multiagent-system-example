using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Globalization;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Gsa
		: Sentence
	{
		public Gsa(string sentence, string talkerId)
			: base(talkerId, TypeCodes.GSA)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			double pdop = 0;
			double hdop = 0;
			double vdop = 0;

			string[] datas = sentence.Split(',');

			if (datas.Length != 18)
				throw new ArgumentException("Sentence format is invalid.", "sentence");

			double.TryParse(datas[15], NumberStyles.Number, CultureInfo.InvariantCulture, out pdop);
			double.TryParse(datas[16], NumberStyles.Number, CultureInfo.InvariantCulture, out hdop);
			double.TryParse(datas[17].Split('*')[0], NumberStyles.Number, CultureInfo.InvariantCulture, out vdop);

			this.Pdop = pdop;
			this.Hdop = hdop;
			this.Vdop = vdop;
		}

		public double Pdop { get; set; }
		public double Hdop { get; set; }
		public double Vdop { get; set; }

		public override void FillGeoData(GeoData data)
		{
			data.PrecisionData.Pdop = this.Pdop;
			data.PrecisionData.Hdop = this.Hdop;
			data.PrecisionData.Vdop = this.Vdop;
		}
	}
}
