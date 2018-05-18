using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class CameraSettings
	{
		public CameraSettings()
		{
			this.CameraMode = CameraMode.Day;
			this.CameraName = "Dummy Camera Name";
			this.IpAddress = "192.168.0.1";
		}

		[JsonProperty]
		public string CameraName { get; private set; }

		[JsonProperty]
		public DateTime LastCameraPhysicalModificationDate { get; protected set; }

		[JsonProperty]
		public CameraMode CameraMode { get; private set; }

		[JsonProperty]
		public string IpAddress { get; private set; }

		[JsonProperty]
		public RegionsOfInterestParameters RegionsOfInterestParameters { get; protected set; }

		[JsonProperty]
		public ImageAnalysisParameters ImageAnalysisParameters { get; protected set; }
	}
}