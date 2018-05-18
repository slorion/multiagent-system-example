using DLC.Scientific.Acquisition.Agents.EventPanelAgent.Configuration;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent
{
	[ServiceContract]
	internal interface IInternalEventPanelAgent
		: IEventPanelAgent
	{
		Orientation UIOrientation { [OperationContract] get; [OperationContract] set; }
		int SplitterDistanceHorizontalMode { [OperationContract] get; [OperationContract] set; }
		int SplitterDistanceVerticalMode { [OperationContract] get; [OperationContract] set; }

		Keys ToggleHotkeyModeKey { [OperationContract] get; [OperationContract] set; }

		[OperationContract]
		Task OnRoadEvent(RoadEventData data, bool isNew = true, double? progress = null);

		[OperationContract]
		void OnHotkeyModeChanged(bool enabled);

		[OperationContract]
		IEnumerable<RoadEvent> ReadRoadEventConfiguration();

		[OperationContract]
		RoadEventData CloneRoadEvent(RoadEventData data);
	}
}