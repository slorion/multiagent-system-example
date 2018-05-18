using System;

namespace DLC.Multiagent
{
	public interface IAgentUI
	{
		event EventHandler<EventArgs> UIClosed;

		void Initialize(IAgent agent);
		void ShowUI();
		void CloseUI();
	}
}