using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding
{
	[DataContract]
	[KnownType(typeof(Gps.PositionData))]
	[Serializable]
	public class GeoCoordinate
	{
		[DataMember]
		public double Altitude { get; set; }

		[DataMember]
		public double Latitude { get; set; }

		[DataMember]
		public double Longitude { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GeoCoordinate"/> class.
		/// </summary>
		public GeoCoordinate()
			: this(0, 0, 0)
		{
		}

		public GeoCoordinate(GeoCoordinate coord)
		{
			if (coord == null) throw new ArgumentNullException("coord");

			this.Longitude = coord.Longitude;
			this.Latitude = coord.Latitude;
			this.Altitude = coord.Altitude;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GeoCoordinate"/> class.
		/// </summary>
		/// <param name="longitude">The longitude.</param>
		/// <param name="latitude">The latitude.</param>
		/// <param name="altitude">The altitude.</param>
		public GeoCoordinate(double longitude, double latitude, double altitude)
		{
			this.Longitude = longitude;
			this.Latitude = latitude;
			this.Altitude = altitude;
		}

		public override string ToString()
		{
			return string.Format("{0};{1};{2}", this.Latitude, this.Longitude, this.Altitude);
		}

		public bool Equals(GeoCoordinate other)
		{
			if (other == null)
				return false;
			else
				return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as GeoCoordinate);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + this.Latitude.GetHashCode();
				hash = hash * 23 + this.Longitude.GetHashCode();
				return hash;
			}
		}

		public static bool operator ==(GeoCoordinate x, GeoCoordinate y)
		{
			if (object.ReferenceEquals(x, y))
				return true;
			else if ((object) x == null || (object) y == null)
				return false;
			else
				return x.Equals(y);
		}

		public static bool operator !=(GeoCoordinate x, GeoCoordinate y)
		{
			return !(x == y);
		}
	}
}