using System;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	public enum NavigationStatus
	{
		Invalid = 0,
		RawMeasurements = 1,
		Initialising = 2,
		Locking = 3,

		/// <summary>
		/// In Locked mode the system is outputting real-time data with the specified
		/// latency guaranteed. All fields are valid.
		/// </summary>
		Locked = 4,

		Unusable = 5,
		ExpiredFirmware = 6,

		StatusOnly = 10,
		InternalUse = 11, //Dot not use any values from this message
		TriggerOutputInitializing = 20,
		TriggerOutputLocking = 21,
		TriggerOutputLocked = 22,
		Others
	}
}