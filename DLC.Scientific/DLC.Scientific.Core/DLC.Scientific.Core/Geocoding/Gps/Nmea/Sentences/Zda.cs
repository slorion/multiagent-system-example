using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Globalization;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Zda
		: Sentence
	{
		public Zda(string sentence, string talkerId)
			: base(talkerId, TypeCodes.ZDA)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");

			TimeSpan utc;
			int day;
			int month;
			int year;
			int hoursOffset;
			int minutesOffset;

			string[] datas = sentence.Split(new[] { ',', '*' });

			if (datas.Length != 8)
				throw new ArgumentException("Sentence format is invalid.", "sentence");

			TimeSpan.TryParseExact(datas[1], "hhmmss\\.FFF", CultureInfo.InvariantCulture, out utc);
			int.TryParse(datas[2], out day);
			int.TryParse(datas[3], out month);
			int.TryParse(datas[4], out year);
			int.TryParse(datas[5], out hoursOffset);
			int.TryParse(datas[6], out minutesOffset);

			string checksum = datas[7];

			this.Utc = utc;
			this.Day = day;
			this.Month = month;
			this.Year = year;
			this.HoursOffset = hoursOffset;
			this.MinutesOffset = minutesOffset;
			this.Checksum = checksum;
		}

		/// <summary>
		/// Gets the checksum.
		/// </summary>
		public string Checksum { get; set; }

		/// <summary>
		/// Gets the day.
		/// </summary>
		public int Day { get; set; }

		/// <summary>
		/// Gets the hours offset.
		/// </summary>
		public int HoursOffset { get; set; }

		/// <summary>
		/// Gets the minutes offset.
		/// </summary>
		public int MinutesOffset { get; set; }

		/// <summary>
		/// Gets the month.
		/// </summary>
		public int Month { get; set; }

		/// <summary>
		/// Gets the UTC time in hours , minutes, seconds of the GPS position.
		/// </summary>
		public TimeSpan Utc { get; set; }

		/// <summary>
		/// Gets the year.
		/// </summary>
		public int Year { get; set; }

		public override void FillGeoData(GeoData data)
		{
			if (this.Year > 0 && this.Month > 0 && this.Day > 0)
			{
				data.PositionData.Utc = new DateTime(
					this.Year,
					this.Month,
					this.Day,
					this.Utc.Hours,
					this.Utc.Minutes,
					this.Utc.Seconds,
					this.Utc.Milliseconds,
					DateTimeKind.Utc);
			}
		}
	}
}
