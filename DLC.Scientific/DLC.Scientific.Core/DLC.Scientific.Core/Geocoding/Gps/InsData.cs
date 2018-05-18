using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	/// <summary>
	/// Represent data coming from an Inertial Navigation System
	/// </summary>
	[DataContract]
	[Serializable]
	public class InsData
	{
		/// <summary>
		/// Gets or sets the acceleration X.
		/// </summary>
		/// <value>
		/// The acceleration X.
		/// </value>
		[DataMember]
		public double AccelerationX { get; set; }

		/// <summary>
		/// Gets or sets the acceleration Y.
		/// </summary>
		/// <value>
		/// The acceleration Y.
		/// </value>
		[DataMember]
		public double AccelerationY { get; set; }

		/// <summary>
		/// Gets or sets the acceleration Z.
		/// </summary>
		/// <value>
		/// The acceleration Z.
		/// </value>
		[DataMember]
		public double AccelerationZ { get; set; }

		/// <summary>
		/// Gets or sets the angular rate X.
		/// </summary>
		/// <value>
		/// The angular rate X.
		/// </value>
		[DataMember]
		public double AngularRateX { get; set; }

		/// <summary>
		/// Gets or sets the angular rate Y.
		/// </summary>
		/// <value>
		/// The angular rate Y.
		/// </value>
		[DataMember]
		public double AngularRateY { get; set; }

		/// <summary>
		/// Gets or sets the angular rate Z.
		/// </summary>
		/// <value>
		/// The angular rate Z.
		/// </value>
		[DataMember]
		public double AngularRateZ { get; set; }

		/// <summary>
		/// Gets or sets down velocity.
		/// </summary>
		/// <value>
		/// Down velocity.
		/// </value>
		[DataMember]
		public double DownVelocity { get; set; }

		/// <summary>
		/// Gets or sets the east velocity.
		/// </summary>
		/// <value>
		/// The east velocity.
		/// </value>
		[DataMember]
		public double EastVelocity { get; set; }

		/// <summary>
		/// Gets or sets the heading.
		/// </summary>
		/// <value>
		/// The heading.
		/// </value>
		[DataMember]
		public double Heading { get; set; }

		/// <summary>
		/// Gets or sets the north velocity.
		/// </summary>
		/// <value>
		/// The north velocity.
		/// </value>
		[DataMember]
		public double NorthVelocity { get; set; }

		/// <summary>
		/// Gets or sets the pitch.
		/// </summary>
		/// <value>
		/// The pitch.
		/// </value>
		[DataMember]
		public double Pitch { get; set; }

		/// <summary>
		/// Gets or sets the roll.
		/// </summary>
		/// <value>
		/// The roll.
		/// </value>
		[DataMember]
		public double Roll { get; set; }

		/// <summary>
		/// The navigation status. In Locked mode the system is outputting real-time data with the specified 
		/// latency guaranteed. All fields are valid.
		/// </summary>
		[DataMember]
		public NavigationStatus Status { get; set; }
	}
}
