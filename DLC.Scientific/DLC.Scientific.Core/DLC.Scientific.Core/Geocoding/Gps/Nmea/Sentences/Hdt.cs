using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Nmea.Sentences
{
	internal sealed class Hdt
		: Sentence
	{
		public Hdt(string sentence, string talkerId)
			: base(talkerId, TypeCodes.HDT)
		{
			if (string.IsNullOrEmpty(sentence)) throw new ArgumentNullException("sentence");
		}
	}
}
