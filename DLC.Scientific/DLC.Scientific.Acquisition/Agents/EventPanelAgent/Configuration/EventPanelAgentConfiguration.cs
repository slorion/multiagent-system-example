using DLC.Scientific.Acquisition.Core.Configuration;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.Configuration
{
	public class EventPanelAgentConfiguration
		: AcquisitionAgentConfiguration
	{
		public bool ShowErrorList { get; set; }
		public string ToggleHotkeyModeKey { get; set; }
		public List<RoadEvent> RoadEvents { get; set; }

		public Orientation UIOrientation { get; set; }
		public int SplitterDistanceHorizontalMode { get; set; }
		public int SplitterDistanceVerticalMode { get; set; }
	}

	public class RoadEvent
	{
		public List<RoadEventDataDisplayInfo> RoadEventDataDisplayInfos { get; set; }
	}
}
