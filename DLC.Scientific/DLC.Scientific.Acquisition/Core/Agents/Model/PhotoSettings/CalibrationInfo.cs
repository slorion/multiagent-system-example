using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class CalibrationInfo
	{
		[DataMember]
		public CameraIntrinsics CameraIntrinsics { get; set; }

		[DataMember]
		public CameraExtrinsics CameraExtrinsics { get; set; }
	}
}