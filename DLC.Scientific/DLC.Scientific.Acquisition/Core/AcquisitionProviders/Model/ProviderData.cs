using DLC.Framework;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class ProviderData
	{
		public ProviderData()
		{
			this.Timestamp = DateTimePrecise.Now;
		}

		[DataMember]
		public DateTime Timestamp { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1:o}", this.GetType().Name, this.Timestamp);
		}
	}
}