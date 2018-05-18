using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	[DataContract]
	[Serializable]
	public class InitializeAcquisitionParameter
		: AcquisitionParameter
	{
		public override AcquisitionStep AcquisitionStep
		{
			get { return AcquisitionStep.Initialize; }
		}
	}
}