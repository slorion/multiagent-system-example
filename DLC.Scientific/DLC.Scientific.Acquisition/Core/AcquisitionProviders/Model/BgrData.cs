using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class BgrData
		: ProviderData
	{
		[DataMember]
		public IRtssc Rtssc { get; set; }

		[DataMember]
		public DirectionBgr Direction { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1}", base.ToString(), this.Rtssc);
		}
	}
}