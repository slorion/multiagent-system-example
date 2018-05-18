using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	[DataContract]
	public class RoutepointExtension
	{
		[DataMember]
		public double? Progress { get; set; }
	}
}
