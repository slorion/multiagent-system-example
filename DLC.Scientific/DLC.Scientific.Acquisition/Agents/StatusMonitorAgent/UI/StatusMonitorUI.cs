using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.StatusMonitorAgent.UI
{
	public partial class StatusMonitorUI
		: AcquisitionStickyForm
	{
		class StateImageInfo
		{
			public int ProviderIconIndex { get; set; }
			public int OperationalIconIndex { get; set; }
			public string ToolTipText { get; set; }
		}

		private readonly ConcurrentDictionary<string, Tuple<AgentInformation, ProviderState>> _displayedAgents = new ConcurrentDictionary<string, Tuple<AgentInformation, ProviderState>>();
		private readonly Dictionary<ProviderState, StateImageInfo> _providerStatesIconIndexAndTooltip = new Dictionary<ProviderState, StateImageInfo>();
		private Size _expandedSize;

		private new IStatusMonitorAgent ParentAgent { get { return (IStatusMonitorAgent) base.ParentAgent; } }

		public StatusMonitorUI()
			: base()
		{
			InitializeComponent();

			_expandedSize = this.Size;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			foreach (var state in Enum.GetValues(typeof(ProviderState)).Cast<ProviderState>())
			{
				var description = ProviderStateHelper.GetStateDescription(state);

				imlStateIcons.Images.Add(ProviderStateHelper.GetStateImage(imlStateIcons.ImageSize.Width, description.Item1));
				imlStateIcons.Images.Add(ProviderStateHelper.GetStateImage(imlStateIcons.ImageSize.Width, description.Item1, Color.FromArgb(255, 64, 64, 64)));
				_providerStatesIconIndexAndTooltip[state] = new StateImageInfo { ProviderIconIndex = imlStateIcons.Images.Count - 2, OperationalIconIndex = imlStateIcons.Images.Count - 1, ToolTipText = description.Item2 };
			}

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<Tuple<SerializableAgentInformation, ProviderState>>(this.ParentAgent.Id, "AgentsProviderStateDataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							_displayedAgents.AddOrUpdate(
								data.Item1.AgentId,
								key =>
								{
									AddProviderAgent(data.Item1, data.Item2);
									return Tuple.Create((AgentInformation) data.Item1, data.Item2);
								},
								(key, old) =>
								{
									UpdateProviderAgent(data.Item1, data.Item2);
									return Tuple.Create((AgentInformation) data.Item1, data.Item2);
								});

							var globalProviderState =
								_displayedAgents
									.Where(a => a.Value.Item2 != ProviderState.Created)
									.GroupBy(
										a =>
										{
											if (a.Value.Item2 < ProviderState.Started) return 1;
											else if (a.Value.Item1.Contracts.Contains(typeof(IAcquisitionableAgent).AssemblyQualifiedName)) return 2;
											else if (a.Value.Item1.Contracts.Contains(typeof(IProviderAgent).AssemblyQualifiedName)) return 3;
											else return 4;
										})
									.OrderBy(g => g.Key)
									.Select(g => g.Min(a => a.Value.Item2))
									.FirstOrDefault();

							var globalStateInfo = _providerStatesIconIndexAndTooltip[globalProviderState];
							this.Icon = _stateIcons[globalProviderState];
						}));

			this.ParentAgent.QueryStateForAllAgents();
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			foreach (Image image in imlStateIcons.Images)
				image.Dispose();
		}

		private void AddProviderAgent(AgentInformation agentInfo, ProviderState state)
		{
			string name = agentInfo.DisplayData.ShortName;
			var indexAndTooltip = _providerStatesIconIndexAndTooltip[state];

			var item = new ListViewItem(name);
			item.Name = name;
			item.ForeColor = Color.Yellow;
			item.UseItemStyleForSubItems = true;

			item.ToolTipText = string.Format("{0} - {1} ({2})", agentInfo.PeerNode.Description, agentInfo.DisplayData.Name, indexAndTooltip.ToolTipText);

			if (!agentInfo.Contracts.Contains(typeof(IProviderAgent).AssemblyQualifiedName))
				item.ImageIndex = indexAndTooltip.OperationalIconIndex;
			else
				item.ImageIndex = indexAndTooltip.ProviderIconIndex;

			lvwProviderAgents.Items.Add(item);
			lvwProviderAgents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void UpdateProviderAgent(AgentInformation agentInfo, ProviderState state)
		{
			string name = agentInfo.DisplayData.ShortName;

			var item = lvwProviderAgents.Items.Find(name, true).FirstOrDefault();
			if (item != null)
			{
				var indexAndTooltip = _providerStatesIconIndexAndTooltip[state];

				item.ToolTipText = string.Format("{0} - {1} ({2})", agentInfo.PeerNode.Description, agentInfo.DisplayData.Name, indexAndTooltip.ToolTipText);

				if (!agentInfo.Contracts.Contains(typeof(IProviderAgent).AssemblyQualifiedName))
					item.ImageIndex = indexAndTooltip.OperationalIconIndex;
				else
					item.ImageIndex = indexAndTooltip.ProviderIconIndex;
			}
		}
	}
}