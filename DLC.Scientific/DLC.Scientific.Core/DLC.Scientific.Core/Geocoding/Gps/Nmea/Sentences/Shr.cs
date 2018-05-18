using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Shr
		: Sentence
	{
		public Shr(string sentence, string talkerId)
			: base(talkerId, TypeCodes.SHR)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");
		}
	}
}
