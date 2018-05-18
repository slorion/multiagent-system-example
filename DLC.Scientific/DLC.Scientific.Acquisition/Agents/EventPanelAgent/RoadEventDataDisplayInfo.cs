using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent
{
	[DataContract]
	[Serializable]
	public class RoadEventDataDisplayInfo
	{
		[DataMember]
		public Keys Hotkey { get; set; }

		[DataMember]
		public Color? CustomColor { get; set; }

		[DataMember]
		public string ImagePath { get; set; }

		[DataMember]
		public bool ShowEditDialog { get; set; }

		[DataMember]
		public RoadEventData RoadEventDataTemplate { get; set; }
	}
}