using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class SpeedData
		: ProviderData
	{
		/// <summary>
		/// Official current speed.
		/// </summary>
		[DataMember]
		public double CurrentSpeed { get; set; }

		/// <summary>
		/// Official speed data source.
		/// </summary>
		[DataMember]
		public SpeedActiveMode SpeedSource { get; set; }

		/// <summary>
		/// Absolute distance when speed was calculated.
		/// </summary>
		[DataMember]
		public double? CurrentDistance { get; set; }

		/// <summary>
		/// Speed from odometer.
		/// </summary>
		[DataMember]
		public double? DistanceSpeed { get; set; }

		/// <summary>
		/// Speed from GPS.
		/// </summary>
		[DataMember]
		public double? GpsSpeed { get; set; }

		[DataMember]
		public bool IsInRange { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1}", base.ToString(), this.CurrentSpeed);
		}
	}
}