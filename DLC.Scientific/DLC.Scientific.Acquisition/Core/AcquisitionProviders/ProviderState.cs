namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public enum ProviderState
	{
		Created = 0,
		Initializing = 100,
		Initialized = 200,
		Starting = 300,
		Started = 400,
		InitializingRecord = 500,
		InitializedRecord = 600,
		StartingRecord = 700,
		StartedRecord = 800,
		StoppingRecord = 650,
		UninitializingRecord = 450,
		Stopping = 250,
		Uninitializing = 50,
		Disposed = 1,
		Failed = 2,
		Calibrating = 10
	}
}