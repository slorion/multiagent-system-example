using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	public abstract class Sentence
	{
		public string TalkerId { get; private set; }
		public string TypeCode { get; private set; }

		public virtual void FillGeoData(GeoData data) { }

		protected Sentence(string talkerId, string typeCode)
		{
			if (string.IsNullOrEmpty(talkerId)) throw new ArgumentNullException("talkerId");
			if (string.IsNullOrEmpty(typeCode)) throw new ArgumentNullException("typeCode");

			TalkerId = talkerId;
			TypeCode = typeCode;
		}

		/// <summary>
		/// Converts the min dec to decimal degree.
		/// </summary>
		/// <param name="nmeaCoord">The nmea coord.</param>
		/// <param name="direction">The direction.</param>
		/// <returns>A representation in decimal degree of nmeaCord</returns>
		protected double ConvertMinDecToDecimalDegree(double nmeaCoord, string direction)
		{
			// NMEA coord is represented as 4807.038,N : Latitude 48 deg 07.038' N
			double degrees = Math.Truncate(nmeaCoord / 100);
			double minSec = (nmeaCoord - (degrees * 100)) / 60;

			double result = degrees + minSec;

			if (direction == "S" || direction == "W")
				result *= -1;

			return result;
		}
	}
}