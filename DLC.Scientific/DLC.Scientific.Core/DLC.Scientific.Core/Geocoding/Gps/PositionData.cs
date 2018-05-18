using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	/// <summary>
	///   Represent a position based on GPS and INS datas.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PositionData
		: GeoCoordinate
	{
		/// <summary>
		/// Gets or sets the ins data.
		/// </summary>
		/// <value>
		/// The ins data.
		/// </value>
		[DataMember]
		public InsData InsData { get; set; }

		/// <summary>
		/// Gets or sets the nb satellites.
		/// </summary>
		/// <value>
		/// The nb satellites.
		/// </value>
		[DataMember]
		public int NbSatellites { get; set; }

		/// <summary>
		/// Gets or sets the quality.
		/// </summary>
		/// <value>
		/// The quality.
		/// </value>
		[DataMember]
		public FixType Quality { get; set; }

		/// <summary>
		/// Gets or sets the UTC.
		/// </summary>
		/// <value>
		/// The UTC.
		/// </value>
		[DataMember]
		public DateTime Utc { get; set; }

		/// <summary>
		/// Age of differential GPS data (in seconds)
		/// </summary>
		[DataMember]
		public double DifferentialDataAge { get; set; }

		/// <summary>
		/// Differential base station ID
		/// </summary>
		[DataMember]
		public string DifferentialStationId { get; set; }

		/// <summary>
		/// Undulation value (difference between RT Altitude and WGS-84 Ellipsoidal Altitude)
		/// </summary>
		[DataMember]
		public double GeoIdHeight { get; set; }

		public bool Equals(PositionData other)
		{
			if (other == null)
				return false;
			else
				return base.Equals(other) && this.Utc == other.Utc;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PositionData);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() * 23 + this.Utc.GetHashCode();
		}

		public static bool operator ==(PositionData x, PositionData y)
		{
			if (object.ReferenceEquals(x, y))
				return true;
			else if ((object) x == null || (object) y == null)
				return false;
			else
				return x.Equals(y);
		}

		public static bool operator !=(PositionData x, PositionData y)
		{
			return !(x == y);
		}
	}

	/// <summary>
	/// Compare only latitude and longitude, not the UTC.
	/// </summary>
	public class PositionLatLongComparer
		: EqualityComparer<PositionData>
	{
		public static readonly PositionLatLongComparer Instance = new PositionLatLongComparer();

		public override bool Equals(PositionData x, PositionData y)
		{
			if (object.ReferenceEquals(x, y))
				return true;
			else if ((object) x == null || (object) y == null)
				return false;
			else
				return x.Latitude == y.Latitude && x.Longitude == y.Longitude;
		}

		public override int GetHashCode(PositionData obj)
		{
			unchecked
			{
				int hash = 17;

				if (obj != null)
				{
					hash = hash * 23 + obj.Latitude.GetHashCode();
					hash = hash * 23 + obj.Longitude.GetHashCode();
				}

				return hash;
			}
		}
	}
}