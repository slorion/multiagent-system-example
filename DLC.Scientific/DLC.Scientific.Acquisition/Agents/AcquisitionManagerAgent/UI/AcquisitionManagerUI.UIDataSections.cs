using System;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	partial class AcquisitionManagerUI
	{
		[Flags]
		private enum UIDataSections
		{
			StartStop = 0x1,
			CurrentRtss = 0x2,
			NextRtss = 0x4,

			All = 0xFFFF
		}
	}
}