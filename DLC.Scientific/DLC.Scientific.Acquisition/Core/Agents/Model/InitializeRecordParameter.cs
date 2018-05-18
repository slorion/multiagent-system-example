using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	[DataContract]
	[Serializable]
	public class InitializeRecordParameter
		: AcquisitionParameter
	{
		public override AcquisitionStep AcquisitionStep
		{
			get { return AcquisitionStep.InitializeRecord; }
		}

		[DataMember]
		public string DefaultRootPath { get; set; }

		[DataMember]
		public string TestType { get; set; }

		[DataMember]
		public string DriverFullName { get; set; }

		[DataMember]
		public string DriverFirstName { get; set; }

		[DataMember]
		public string DriverLastName { get; set; }

		[DataMember]
		public string OperatorFullName { get; set; }

		[DataMember]
		public string OperatorFirstName { get; set; }

		[DataMember]
		public string OperatorLastName { get; set; }

		[DataMember]
		public string VehicleFullName { get; set; }

		[DataMember]
		public string VehicleId { get; set; }

		[DataMember]
		public string VehicleName { get; set; }

		[DataMember]
		public string VehicleType { get; set; }
	}
}