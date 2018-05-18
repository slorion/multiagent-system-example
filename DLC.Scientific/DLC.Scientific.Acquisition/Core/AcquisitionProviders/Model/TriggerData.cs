using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class TriggerData
		: ProviderData
	{
		[DataMember]
		public TriggerMode TriggerMode { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1}", base.ToString(), this.TriggerMode);
		}
	}
}