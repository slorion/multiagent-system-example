using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Rmc
		: Sentence
	{
		public Rmc(string sentence, string talkerId)
			: base(talkerId, TypeCodes.RMC)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");
		}
	}
}