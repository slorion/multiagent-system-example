using DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences;
using System;
using System.Globalization;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea
{
	// see http://www.catb.org/gpsd/NMEA.html (non official reference)
	public class NmeaRawData
	{
		private const char ChecksumDelimiter = '*';

		public string TalkerId { get; private set; }
		public string TypeCode { get; private set; }
		public Sentence Sentence { get; private set; }

		public static bool TryParse(string sentence, out NmeaRawData data)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			if (ValidateChecksum(sentence))
			{
				data = new NmeaRawData(sentence);
				return true;
			}
			else
			{
				data = null;
				return false;
			}
		}

		private NmeaRawData(string sentence)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			this.TalkerId = GetTalkerId(sentence);
			this.TypeCode = GetTypeCode(sentence);

			if (this.TypeCode == TypeCodes.GGA)
				this.Sentence = new Gga(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.GSA)
				this.Sentence = new Gsa(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.GST)
				this.Sentence = new Gst(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.HDT)
				this.Sentence = new Hdt(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.RMC)
				this.Sentence = new Rmc(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.SHR)
				this.Sentence = new Shr(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.VTG)
				this.Sentence = new Vtg(sentence, this.TalkerId);
			else if (this.TypeCode == TypeCodes.ZDA)
				this.Sentence = new Zda(sentence, this.TalkerId);
		}

		public void FillGeoData(GeoData data)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (this.Sentence == null) return;

			data.DeviceType = GpsDeviceType.Gps;
			this.Sentence.FillGeoData(data);
		}

		private static bool ValidateChecksum(string sentence)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			int checksumMarkerIndex = sentence.LastIndexOf(ChecksumDelimiter);
			byte checksum = byte.Parse(sentence.Substring(checksumMarkerIndex + 1), NumberStyles.HexNumber);

			byte actual = 0;
			for (int i = 1; i < sentence.LastIndexOf(ChecksumDelimiter); i++)
				actual ^= (byte) sentence[i];

			return actual == checksum;
		}

		private static string GetTalkerId(string sentence)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			if (sentence.Length < 3)
				throw new ArgumentException("Invalid sentence: " + sentence);

			return sentence.Substring(1, 2);
		}

		private static string GetTypeCode(string sentence)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			if (sentence.Length < 6)
				throw new ArgumentException("Invalid sentence: " + sentence);

			return sentence.Substring(3, 3);
		}
	}
}