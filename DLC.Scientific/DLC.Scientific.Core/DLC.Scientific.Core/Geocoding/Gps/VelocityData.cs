using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	[DataContract]
	[Serializable]
	public class VelocityData
		: IEquatable<VelocityData>
	{
		private double _speedMs = 0.0;

		public VelocityData()
		{
		}

		public VelocityData(double speedMs)
		{
			_speedMs = speedMs;
		}

		[DataMember]
		public double SpeedKmh
		{
			get { return _speedMs * 3.6; }
			set { _speedMs = value / 3.6; }
		}

		[DataMember]
		public double SpeedMs
		{
			get { return _speedMs; }
			set { _speedMs = value; }
		}

		public bool Equals(VelocityData other)
		{
			if (other == null)
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return this.SpeedMs == other.SpeedMs;
		}

		public override int GetHashCode()
		{
			return this.SpeedMs.GetHashCode();
		}
	}
}