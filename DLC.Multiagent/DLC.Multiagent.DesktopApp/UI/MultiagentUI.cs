using DLC.Framework.Reactive;
using DLC.Framework.UI;
using DLC.Multiagent.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Themes;
using Telerik.WinControls.UI;

namespace DLC.Multiagent.DesktopApp.UI
{
	internal partial class MultiagentUI
		: ReactiveForm
	{
		internal protected class UITheme
		{
			public DisplayMode DisplayMode { get; set; }
			public RadThemeComponentBase Theme { get; set; }
			public Color BackColor { get; set; }
			public Color ForeColor { get; set; }
		}

		internal protected enum DisplayMode { Day, Night }

		private readonly SynchronizationContext _mainSynchronizationContext;
		private readonly string _brokerConfigurationFilePath;
		private readonly DataTable _agents;
		private readonly ConcurrentDictionary<string, Tuple<SynchronizationContext, IAgentUI>> _agentUIs = new ConcurrentDictionary<string, Tuple<SynchronizationContext, IAgentUI>>();

		private readonly Dictionary<DisplayMode, UITheme> _uiThemes =
			new Dictionary<DisplayMode, UITheme>{
				{ DisplayMode.Day, new UITheme { DisplayMode = DisplayMode.Day, Theme = new TelerikMetroTheme(), BackColor = default(Color), ForeColor = default(Color) } },
				{ DisplayMode.Night, new UITheme { DisplayMode = DisplayMode.Night, Theme = new VisualStudio2012DarkTheme(), BackColor = Color.DarkGray, ForeColor = Color.White } },
			};

		private readonly BehaviorSubjectSlim<UITheme> _uiThemeSubject;

		protected IObservable<UITheme> UIThemeDataSource { get { return _uiThemeSubject; } }

		public MultiagentUI()
		{
			InitializeComponent();

			_mainSynchronizationContext = SynchronizationContext.Current;
			_uiThemeSubject = new BehaviorSubjectSlim<UITheme>(_uiThemes[DisplayMode.Night]);

			txtInformation.TextChanged += (s, e) => this.Text = string.Format("Multiagent v{0} (Legendary Edition) - {1}", this.GetType().Assembly.GetName().Version, txtInformation.Text);

			// http://www.iconarchive.com/show/oxygen-icons-by-oxygen-icons.org.html

			btnServiceStartStop.FlatAppearance.BorderSize = 0;
			btnServiceConfigure.FlatAppearance.BorderSize = 0;
			btnServiceShowLog.FlatAppearance.BorderSize = 0;
			btnDisplayMode.FlatAppearance.BorderSize = 0;
			btnAgentActivate.FlatAppearance.BorderSize = 0;
			btnAgentDeactivate.FlatAppearance.BorderSize = 0;
			btnAgentRecycle.FlatAppearance.BorderSize = 0;
			btnAgentShowMainGui.FlatAppearance.BorderSize = 0;
			btnAgentCloseAllGui.FlatAppearance.BorderSize = 0;

			btnAgentActivate.Image = ImageResources.AgentActivate;
			btnAgentDeactivate.Image = ImageResources.AgentDeactivate;
			btnAgentRecycle.Image = ImageResources.AgentRecycle;
			btnAgentShowMainGui.Image = ImageResources.AgentShowGui;
			btnAgentCloseAllGui.Image = ImageResources.AgentCloseGui;

			btnServiceConfigure.Image = ImageResources.ServiceConfigure;
			btnServiceShowLog.Image = ImageResources.ServiceShowLog;
			btnDisplayMode.Image = ImageResources.ServiceDisplayDay;

			var stateDescriptionExpr = string.Format(
				"IIF(reachable=false,'Disconnected', IIF(state={0},'Created', IIF(state={1},'Ready', IIF(state={2},'Activating', IIF(state={3},'Activated', IIF(state={4},'Deactivating', IIF(state={5},'Disposed', IIF(state={6},'Failed', 'Unknown'))))))))",
				(int) AgentState.Created,
				(int) AgentState.Idle,
				(int) AgentState.Activating,
				(int) AgentState.Activated,
				(int) AgentState.Deactivating,
				(int) AgentState.Disposed,
				(int) AgentState.Failed);

			_agents = new DataTable("agents");
			_agents.Columns.Add(new DataColumn("peer", typeof(string)) { Caption = "Peer" });
			_agents.Columns.Add(new DataColumn("id", typeof(string)) { Caption = "ID" });
			_agents.Columns.Add(new DataColumn("name", typeof(string)) { Caption = "Name" });
			_agents.Columns.Add(new DataColumn("description", typeof(string)) { Caption = "Description" });
			_agents.Columns.Add(new DataColumn("isInternal", typeof(bool)) { Caption = "IsInternal (internal)" });
			_agents.Columns.Add(new DataColumn("state", typeof(AgentState)) { Caption = "AgentState (internal)" });
			_agents.Columns.Add(new DataColumn("reachable", typeof(bool)) { Caption = "Reachable (internal)" });
			_agents.Columns.Add(new DataColumn("stateDescription", typeof(string), stateDescriptionExpr) { Caption = "State" });
			_agents.PrimaryKey = new[] { _agents.Columns["id"] };

			gridAgents.DataSource = _agents.DefaultView;
			gridAgents.Columns["id"].IsVisible = false;
			gridAgents.Columns["isInternal"].IsVisible = false;
			gridAgents.Columns["reachable"].IsVisible = false;
			gridAgents.Columns["state"].IsVisible = false;

			gridAgents.SortDescriptors.Add("id", ListSortDirection.Ascending);
			gridAgents.GroupDescriptors.Add("peer", ListSortDirection.Ascending);

			// completely load every themes now to avoid deadlocking (in ThemeRepository.FindTheme)
			// when loading a new theme while multiple forms are opened on 1+N different UI threads
			foreach (var theme in _uiThemes)
				ThemeRepository.FindTheme(theme.Value.Theme.ThemeName);

			TelerikHelper.RegisterTooltipForRadControl(this, ttMain, gridAgents);
		}

		public MultiagentUI(string brokerConfigurationFilePath)
			: this()
		{
			if (string.IsNullOrEmpty(brokerConfigurationFilePath))
				brokerConfigurationFilePath = AgentBrokerConfiguration.DefaultConfigurationFilePath;

			_brokerConfigurationFilePath = brokerConfigurationFilePath;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.RegisterObserver(
				this.UIThemeDataSource
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						uiTheme =>
						{
							gridAgents.ThemeName = uiTheme.Theme.ThemeName;
							this.BackColor = uiTheme.BackColor;
							txtInformation.BackColor = uiTheme.BackColor;
							txtInformation.ForeColor = uiTheme.ForeColor;
						}));

			this.RegisterObserver(
				AgentBroker.Instance.StateDataSource
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						state =>
						{
							switch (state)
							{
								case ServiceState.Starting:
									txtInformation.Text = "Starting";
									btnServiceStartStop.Enabled = false;
									break;
								case ServiceState.Started:
									ttMain.SetToolTip(btnServiceStartStop, "Stop service");
									txtInformation.Text = string.Format("Started ({0} - {1}:{2})", AgentBroker.Instance.LocalPeerNode.Description, AgentBroker.Instance.LocalPeerNode.Host, AgentBroker.Instance.LocalPeerNode.Port);

									btnServiceStartStop.Image = ImageResources.ServiceStop;
									btnServiceStartStop.Enabled = true;
									break;
								case ServiceState.Stopping:
									txtInformation.Text = "Stopping";
									btnServiceStartStop.Enabled = false;
									break;
								case ServiceState.Stopped:
									_agentUIs.Clear();
									_agents.Clear();

									// grid does not clear selection automatically
									gridAgents.ClearSelection();

									ttMain.SetToolTip(btnServiceStartStop, "Start service");
									txtInformation.Text = "Not started";

									btnServiceStartStop.Image = ImageResources.ServiceStart;
									btnServiceStartStop.Enabled = true;
									break;
							}
						}));

			this.RegisterObserver(
				AgentBroker.Instance.AgentDataSource
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						info =>
						{
							var row = _agents.Rows.Find(info.AgentId);

							if (row == null)
								row = _agents.NewRow();

							row["peer"] = info.PeerNode.ToString();
							row["name"] = info.DisplayData.Name;
							row["description"] = info.DisplayData.Description;
							row["isInternal"] = info.IsInternal;
							row["state"] = info.LastKnownState;
							row["reachable"] = info.IsReachable;

							if (row.RowState == DataRowState.Detached)
							{
								row["id"] = info.AgentId;
								_agents.Rows.Add(row);
							}

							if (info.LastKnownState == AgentState.Disposed || info.IsRecycled)
							{
								Tuple<SynchronizationContext, IAgentUI> agentUI;
								if (_agentUIs.TryRemove(info.AgentId, out agentUI))
									UIThreadingHelper.DispatchUI(agentUI.Item2.CloseUI, agentUI.Item1);
							}
						}));

			this.RegisterObserver(
				AgentBroker.Instance.AgentDataSource
					.Where(info => info.LastKnownState == AgentState.Activated)
					.Subscribe(
						info =>
						{
							var getAgentResult = AgentBroker.Instance.TryGetAgent<IVisibleAgent>(info.AgentId);

#pragma warning disable 4014
							if (getAgentResult.Item1 == TryGetAgentResult.Success && getAgentResult.Item2.IsLocal && getAgentResult.Item3.AutoShowUI)
								ShowAgentUI(info.AgentId, getAgentResult.Item3);
#pragma warning restore 4014
						}));

			ErrorHandler.Try(
					() => AgentBroker.Instance.LoadConfiguration(_brokerConfigurationFilePath),
					"Configuration load error.",
					"An error occurred while loading configuration.",
					canRetry: true)
				.ContinueWith(
					t =>
					{
						if (t.IsCompleted && t.Result)
						{
							if (AgentBroker.Instance.Configuration.AutoShowLogWindow)
								ShowLogWindow();

							if (AgentBroker.Instance.Configuration.AutoStartBroker)
								StartStopService();
						}
					}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			Task.Run(() => AgentBroker.Instance.Stop()).Wait(TimeSpan.FromSeconds(10));
			Application.Exit();
		}

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			StartStopService();
		}

		private void btnServiceConfigure_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo(_brokerConfigurationFilePath) { UseShellExecute = true });
		}

		private void btnServiceShowLog_Click(object sender, EventArgs e)
		{
			ShowLogWindow();
		}

		private void btnDisplayMode_Click(object sender, EventArgs e)
		{
			if (_uiThemeSubject.Value.DisplayMode == DisplayMode.Day)
			{
				btnDisplayMode.Image = ImageResources.ServiceDisplayDay;
				_uiThemeSubject.OnNext(_uiThemes[DisplayMode.Night]);
			}
			else
			{
				btnDisplayMode.Image = ImageResources.ServiceDisplayNight;
				_uiThemeSubject.OnNext(_uiThemes[DisplayMode.Day]);
			}
		}

		private void btnAgentActivate_Click(object sender, EventArgs e)
		{
			ActivateSelectedAgents();
		}

		private void btnAgentDeactivate_Click(object sender, EventArgs e)
		{
			DeactivateSelectedAgents();
		}

		private void btnAgentRecycle_Click(object sender, EventArgs e)
		{
			RecycleSelectedAgents();
		}

		private void btnAgentShowMainGui_Click(object sender, EventArgs e)
		{
			ShowSelectedAgents();
		}

		private void btnAgentCloseAllGui_Click(object sender, EventArgs e)
		{
			foreach (var agentId in GetSelectedAgentIds())
			{
				Tuple<SynchronizationContext, IAgentUI> agentUI;
				if (_agentUIs.TryRemove(agentId, out agentUI))
					UIThreadingHelper.DispatchUI(agentUI.Item2.CloseUI, agentUI.Item1);
			}
		}

		private void gridAgents_CellFormatting(object sender, CellFormattingEventArgs e)
		{
			if (e.CellElement is GridDataCellElement)
			{
				e.CellElement.ToolTipText = e.CellElement.Text;

				if (string.Equals(e.Column.FieldName, "stateDescription", StringComparison.Ordinal))
				{
					e.CellElement.DrawImage = true;
					e.CellElement.DrawText = false;

					var row = ((DataRowView) e.Row.DataBoundItem).Row;

					if (!(bool) row["reachable"])
						e.CellElement.Image = ImageResources.AgentStateError;
					else
					{
						switch ((AgentState) row["state"])
						{
							case AgentState.Created:
							case AgentState.Idle:
								e.CellElement.Image = ImageResources.AgentStateIdle;
								break;
							case AgentState.Activating:
							case AgentState.Deactivating:
								e.CellElement.Image = ImageResources.AgentStateWaiting;
								break;
							case AgentState.Activated:
								e.CellElement.Image = ImageResources.AgentStateActivated;
								break;
							case AgentState.Disposed:
							case AgentState.Failed:
								e.CellElement.Image = ImageResources.AgentStateError;
								break;
						}
					}
				}
				else
				{
					e.CellElement.DrawImage = false;
					e.CellElement.DrawText = true;
				}
			}
		}

		private void gridAgents_CellDoubleClick(object sender, GridViewCellEventArgs e)
		{
			if (!Control.ModifierKeys.HasFlag(Keys.Alt))
			{
				ActivateSelectedAgents();
			}
			else if (e.Row is GridViewDataRowInfo)
			{
				var row = (DataRowView) e.Row.DataBoundItem;
				if (row != null)
				{
					Task.Run(
						async () =>
						{
							var result = await AgentBroker.Instance.TryExecuteOnOne<IAgent, string>((string) row["id"], a => a.ConfigurationFilePath, ignoreAgentState: true).ConfigureAwait(false);
							if (result.IsSuccessful && !string.IsNullOrEmpty(result.Result))
								Process.Start(new ProcessStartInfo(result.Result) { UseShellExecute = true });
						});
				}
			}
		}

		private Task StartStopService()
		{
			if (AgentBroker.Instance.State == ServiceState.Started)
			{
				return Task.WhenAll(_agentUIs.Values.Select(agentUI => Task.Run(() => UIThreadingHelper.DispatchUI(agentUI.Item2.CloseUI, agentUI.Item1, asynchronous: false))))
					.ContinueWith(t => AgentBroker.Instance.Stop())
					.ContinueWith(
						t =>
						{
							if (t.IsFaulted)
								MessageBox.Show(t.Exception.ToString(), "Stop has failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
							else if (t.IsCanceled)
								MessageBox.Show(t.Exception.ToString(), "Stop canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else if (AgentBroker.Instance.State == ServiceState.Stopped)
			{
				AgentBroker.Instance.LoadConfiguration(_brokerConfigurationFilePath);

				PlaySound("startup");

				return AgentBroker.Instance.Start()
					.ContinueWith(
						t =>
						{
							if (t.IsFaulted)
								MessageBox.Show(t.Exception.ToString(), "Start has failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
							else if (t.IsCanceled)
								MessageBox.Show(string.Format("Start has been canceled: {0}", t.Exception), "Start canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			else
			{
				throw new InvalidOperationException(string.Format("Service state must be '{0}' or '{1}', but current state is '{2}'.", ServiceState.Started, ServiceState.Stopped, AgentBroker.Instance.State));
			}
		}

		private Task ExecuteOnSelectedAgents<TAgent>(Func<string, TAgent, Task> operation)
			where TAgent : IAgent
		{
			if (operation == null) throw new ArgumentNullException("operation");

			var selected = GetSelectedAgentIds().ToArray();

			return Task.Run(() => Task.WhenAll(selected.Select(agentId => AgentBroker.Instance.TryExecuteOnOne<TAgent>(agentId, a => operation(agentId, a), ignoreAgentState: true))))
				.ContinueWith(
					t =>
					{
						foreach (var result in t.Result.Where(r => r.Exception != null))
							MessageBox.Show(result.Exception.ToString(), string.Format("Execution error on agent '{0}'", result.AgentId), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private Task ActivateSelectedAgents()
		{
			if (gridAgents.SelectedRows.Count > 0)
				PlaySound("activate");

			return ExecuteOnSelectedAgents<IAgent>((agentId, a) => a.Activate());
		}

		private Task DeactivateSelectedAgents()
		{
			return ExecuteOnSelectedAgents<IAgent>((agentId, a) => a.Deactivate());
		}

		private Task RecycleSelectedAgents()
		{
			var selected = GetSelectedAgentIds().ToArray();

			if (selected.Length > 0)
				PlaySound("recycle");

			return Task.Run(() => Task.WhenAll(AgentBroker.Instance.RecycleAgent(selected)));
		}

		private Task ShowSelectedAgents()
		{
			return ExecuteOnSelectedAgents<IVisibleAgent>(ShowAgentUI);
		}

		private IEnumerable<string> GetSelectedAgentIds()
		{
			return
				from row in gridAgents.SelectedRows
				let record = (DataRowView) row.DataBoundItem
				where !(bool) record["isInternal"]
				select (string) record["id"];
		}

		private async Task ShowAgentUI(string agentId, IVisibleAgent agent)
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (agent == null) throw new ArgumentNullException("agent");

			Tuple<SynchronizationContext, IAgentUI> agentUI;

			if (_agentUIs.TryGetValue(agentId, out agentUI))
			{
				UIThreadingHelper.DispatchUI(agentUI.Item2.ShowUI, agentUI.Item1);
			}
			else
			{
				var uiAgentTypeNameResult = await AgentBroker.Instance.TryExecuteOnOne<IVisibleAgent, string>(agentId, a => a.MainUIAgentTypeName).ConfigureAwait(false);
				if (!uiAgentTypeNameResult.IsSuccessful)
				{
					UIThreadingHelper.DispatchUI(() => MessageBox.Show(this, string.Format("The UI cannot be shown because of a communication error with agent '{0}'.\n\n{1}", agentId, uiAgentTypeNameResult.Exception), "Show UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error), _mainSynchronizationContext);
					return;
				}

				var getAgentResult = AgentBroker.Instance.TryGetAgent(Type.GetType(uiAgentTypeNameResult.Result), agentId);
				if (getAgentResult.Item1 != TryGetAgentResult.Success)
				{
					UIThreadingHelper.DispatchUI(() => MessageBox.Show(this, string.Format("The UI cannot be shown because of an internal error from agent '{0}' ('{1}').", agentId, getAgentResult), "Show UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error), _mainSynchronizationContext);
					return;
				}

				var uiTypeNameResult = await AgentBroker.Instance.TryExecuteOnOne<IVisibleAgent, string>(agentId, a => a.MainUITypeName).ConfigureAwait(false);
				if (!uiTypeNameResult.IsSuccessful)
				{
					UIThreadingHelper.DispatchUI(() => MessageBox.Show(this, string.Format("The UI cannot be shown because of a communication error with agent '{0}'.\n\n{1}", agentId, uiTypeNameResult.Exception), "Show UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error), _mainSynchronizationContext);
					return;
				}

				var guiType = Type.GetType(uiTypeNameResult.Result);
				if (guiType == null)
				{
					UIThreadingHelper.DispatchUI(() => MessageBox.Show(this, string.Format("The UI cannot be shown because the library for agent '{0}' is not available locally.", agentId), "Show UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error), _mainSynchronizationContext);
					return;
				}

				UIThreadingHelper.RunInNewUIThread(
					() =>
					{
						var ui = (IAgentUI) Activator.CreateInstance(guiType);
						_agentUIs.TryAdd(agentId, Tuple.Create(SynchronizationContext.Current, ui));

						ui.UIClosed += (s2, e2) => _agentUIs.TryRemove(agentId, out agentUI);
						ui.Initialize(getAgentResult.Item3);
						ui.ShowUI();
					});
			}
		}

		private void ShowLogWindow()
		{
			UIThreadingHelper.RunInNewUIThread(() => new LoggingDialog(this.UIThemeDataSource).ShowDialog());
		}

		#region Easter Egg

		private bool _soundActivatedSC;
		private bool _soundActivatedWC;
		private readonly Random _soundRandom = new Random();
		private readonly IEnumerable<string> _sounds = typeof(MultiagentUI).Assembly.GetManifestResourceNames().Where(path => path.Contains("_sounds"));

		private void MultiagentUI_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Control && e.Shift)
			{
				if (e.KeyCode == Keys.S)
				{
					_soundActivatedSC = !_soundActivatedSC;
					_soundActivatedWC = false;
				}
				else if (e.KeyCode == Keys.W)
				{
					_soundActivatedSC = false;
					_soundActivatedWC = !_soundActivatedWC;
				}
			}
		}

		private void PlaySound(string category)
		{
			var folder = _soundActivatedSC ? "_sounds.sc." : _soundActivatedWC ? "_sounds.wc." : null;

			if (folder != null)
			{
				folder += category;

				try
				{
					var files = _sounds.Where(path => path.Contains(folder)).ToArray();

					if (files.Length > 0)
					{
						using (var stream = typeof(MultiagentUI).Assembly.GetManifestResourceStream(files[_soundRandom.Next(files.Length)]))
						{
							var player = new SoundPlayer(stream);
							player.Play();
						}
					}
				}
				catch { }
			}
		}

		#endregion
	}
}