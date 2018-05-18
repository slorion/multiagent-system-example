using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class CameraExtrinsics
	{
		[DataMember]
		public double[][] RotationMatrix { get; set; }

		[DataMember]
		public double[][] TranslationVector { get; set; }
	}
}