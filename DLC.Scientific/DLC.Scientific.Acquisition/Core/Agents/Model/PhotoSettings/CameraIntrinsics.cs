using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class CameraIntrinsics
	{
		[DataMember]
		public double[][] IntrinsicMatrix { get; set; }

		[DataMember]
		public LensDistorsion LensDistorsion { get; set; }
	}
}