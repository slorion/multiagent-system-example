using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent
{
	[ServiceContract]
	internal interface IInternalAcquisitionManagerAgent : IAcquisitionManagerAgent
	{
		List<string> Drivers { [OperationContract] get; }
		List<string> Operators { [OperationContract] get; }
		List<Tuple<string, string>> Vehicles { [OperationContract] get; }
		List<string> SequenceTypes { [OperationContract] get; }
		Dictionary<int, string> ProximityRanges { [OperationContract] get; }
		string SelectedDriver { [OperationContract] get; }
		string SelectedOperator { [OperationContract] get; }
		string SelectedSequenceType { [OperationContract] get; }
		double StartRtsscSearchRadiusInMeters { [OperationContract] get; }
		double StopRtsscSearchRadiusInMeters { [OperationContract] get; }
		AcquisitionTriggerMode LastSelectedStartTriggerMode { [OperationContract] get; }
		AcquisitionTriggerMode LastSelectedStopTriggerMode { [OperationContract] get; }

		[OperationContract]
		Dictionary<AcquisitionTriggerMode, Tuple<string, string>> GetAcquisitionTriggerModes(bool isStartMode);

		[OperationContract]
		void SaveDriversToConfig(IEnumerable<string> fullNames, string selectedFullName);
		[OperationContract]
		void SaveOperatorsToConfig(IEnumerable<string> fullNames, string selectedFullName);
		[OperationContract]
		void SaveSequenceTypeToConfig(string selectedSequenceType);
	}
}