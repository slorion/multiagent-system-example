using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class LensDistorsion
	{
		[DataMember]
		public double[][] RadialDistorsion { get; set; }
		
		[DataMember]
		public double[][] TangentialDistorsion { get; set; }
	}
}