using DLC.Framework.UI.Forms;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Core.Agents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using Telerik.WinControls;

namespace DLC.Scientific.Acquisition.Core.UI
{
	public partial class AcquisitionStickyForm
		: StickyForm, IAgentUI
	{
		private string _windowTitle;
		private readonly List<IDisposable> _observers = new List<IDisposable>();
		protected readonly Dictionary<ProviderState, Icon> _stateIcons = new Dictionary<ProviderState, Icon>();

		protected IOperationalAgent ParentAgent { get; private set; }

		public AcquisitionStickyForm() : this(null) { }
		public AcquisitionStickyForm(string windowTitle)
			: base()
		{
			InitializeComponent();

			_windowTitle = windowTitle;

			this.IsSticky = true;
			this.StickGap = 20;
			this.IsWindowPositionSavedOnClose = true;
			this.IsWindowSizeSavedOnClose = true;
		}

		#region Form's overrride

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!string.IsNullOrEmpty(_windowTitle))
				this.Text = _windowTitle;
			else if (!this.InVSDesigner())
				this.Text = this.ParentAgent.DisplayData.Name;

			radCollapsiblePanel1.HeaderText = "v" + this.GetType().Assembly.GetName().Version.ToString();

			if (!this.InVSDesigner())
			{
				foreach (var state in Enum.GetValues(typeof(ProviderState)).Cast<ProviderState>().OrderBy(state => (int) state))
					_stateIcons[state] = Icon.FromHandle(ProviderStateHelper.GetStateImage(64, state).GetHicon());

				if (this.ParentAgent is IProviderAgent)
				{
					this.RegisterObserver(
						AgentBroker.Instance.ObserveOne<ProviderState>(this.ParentAgent.Id, "ProviderStateDataSource", ignoreAgentState: true)
							.ObserveOn(WindowsFormsSynchronizationContext.Current)
							.Subscribe(state => this.Icon = _stateIcons[state]));
				}

				_collaspsiblePanel1Height = this.radCollapsiblePanel1.Height;
				if (this.ParentAgent is IVisibleAgent && ((IVisibleAgent) this.ParentAgent).ShowErrorListOnLoad)
					radCollapsiblePanel1.Expand();
				else
					radCollapsiblePanel1.Collapse();

				radCollapsiblePanel1.ControlsContainer.PanelElement.Border.Visibility = ElementVisibility.Collapsed;
				radCollapsiblePanel1.Enabled = false;
			}
		}
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			// close panel before StickyForm saves the window size
			if (!radCollapsiblePanel1.IsDisposed)
				radCollapsiblePanel1.Collapse();

			base.OnFormClosing(e);

			if (!e.Cancel)
			{
				foreach (var observer in _observers)
					observer.Dispose();
				_observers.Clear();
			}
		}
		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			foreach (var icon in _stateIcons)
				icon.Value.Dispose();

			var handler = UIClosed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		#endregion

		#region IAgentUI members

		public event EventHandler<EventArgs> UIClosed;

		public void Initialize(IAgent agent)
		{
			if (agent == null) throw new ArgumentNullException("agent");

			this.ParentAgent = (IOperationalAgent) agent;
			this.Text = agent.DisplayData.Name;

			InitializeCore(agent);
		}
		public virtual void InitializeCore(IAgent agent) { }

		public void ShowUI()
		{
			if (this.WindowState == FormWindowState.Minimized)
				this.WindowState = FormWindowState.Normal;

			this.Activate();

			if (this.Visible)
				this.Show();
			else
				this.ShowDialog();
		}

		public void CloseUI()
		{
			this.Close();
			this.Dispose();
		}

		#endregion

		protected void RegisterObserver(IDisposable observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");
			this._observers.Add(observer);
		}

		private int _collaspsiblePanel1Height;
		private void radCollapsiblePanel1_Expanded(object sender, EventArgs e)
		{
			this.Height += (_collaspsiblePanel1Height - 25);
			this.pnlBaseBottom.Height += (_collaspsiblePanel1Height - 25);
			radCollapsiblePanel1.HeaderText = "";
		}
		private void radCollapsiblePanel1_Collapsed(object sender, EventArgs e)
		{
			this.pnlBaseBottom.Height -= (_collaspsiblePanel1Height - 25);
			this.Height -= (_collaspsiblePanel1Height - 25);
		}
	}
}