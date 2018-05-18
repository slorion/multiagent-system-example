using System;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	public class ExtensionData
	{
		public DateTime ComputerDateTime { get; set; }
		public double CorrectedLatitude { get; set; }
		public double CorrectedLongitude { get; set; }
		public String GpsStatus { get; set; }
		public double Speed { get; set; }
		public double Progress { get; set; }
	}
}