using DLC.Scientific.Acquisition.Core.Configuration;
using System.Collections.Generic;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.Configuration
{
	public class AcquisitionManagerAgentConfiguration
		: AcquisitionAgentConfiguration
	{
		public string DefaultRootPath { get; set; }
		public List<string> Drivers { get; set; }
		public string SelectedDriver { get; set; }
		public List<string> Operators { get; set; }
		public string SelectedOperator { get; set; }
		public List<VehicleDescription> Vehicles { get; set; }
		public string SelectedVehicle { get; set; }
		public List<string> SequenceTypes { get; set; }
		public string SelectedSequenceType { get; set; }

		public List<TriggerDescription> StartTriggerModes { get; set; }
		public string LastSelectedStartTriggerMode { get; set; }
		public List<TriggerDescription> StopTriggerModes { get; set; }
		public string LastSelectedStopTriggerMode { get; set; }

		public int StartStopTriggerRadius { get; set; }
		public bool EnableUseOfOdometricCompensationAlgorithm { get; set; }
		public double MinDistanceToSwitchToGps { get; set; }
		public List<int> ProximityRanges { get; set; }
		public int MinimumSpeed { get; set; }
		public int MaximumSpeed { get; set; }
	}

	public class VehicleDescription
	{
		public string Name { get; set; }
		public string Number { get; set; }
		public string Type { get; set; }
	}

	public class TriggerDescription
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Enabled { get; set; }
	}

	public class ProxymityRangeDescription
	{
		public int Value { get; set; }
		public string Description { get; set; }
	}
}