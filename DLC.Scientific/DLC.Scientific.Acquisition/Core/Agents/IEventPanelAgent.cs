using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	public interface IEventPanelAgent
		: IProviderAgent<RoadEventData>, IAcquisitionableAgent
	{
		IObservable<bool> HotkeyModeEnabledDataSource { get; }
	}
}