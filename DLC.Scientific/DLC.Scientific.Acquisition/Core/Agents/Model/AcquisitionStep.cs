namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	public enum AcquisitionStep
	{
		Initialize,
		Start,
		InitializeRecord,
		StartRecord,
		StopRecord,
		ValidateRecord,
		UninitializeRecord,
		Stop,
		Uninitialize
	}
}