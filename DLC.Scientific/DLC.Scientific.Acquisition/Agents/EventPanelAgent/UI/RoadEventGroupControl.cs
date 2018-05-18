using DLC.Framework;
using DLC.Framework.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI
{
	internal partial class RoadEventGroupControl
		: UserControl
	{
		private readonly Dictionary<RoadEventDataDisplayInfo, Tuple<Image, Image>> _icons = new Dictionary<RoadEventDataDisplayInfo, Tuple<Image, Image>>();

		public event EventHandler<EventArgs<RoadEventDataDisplayInfo>> RoadEventClicked;

		public RoadEventGroupControl(IEnumerable<RoadEventDataDisplayInfo> roadEventGroup)
		{
			if (roadEventGroup == null) throw new ArgumentNullException("roadEventGroup");
			if (!roadEventGroup.Any()) throw new ArgumentException("roadEventGroup must contain at least one element.", "roadEventGroup");

			InitializeComponent();

			foreach (var roadEvent in roadEventGroup)
			{
				Image active = new Bitmap(Image.FromFile(roadEvent.ImagePath), picIcon.Size);
				Image inactive = ImageHelper.MakeGrayscale(active);
				_icons[roadEvent] = Tuple.Create(active, inactive);
			}

			this.RoadEventGroup = new ReadOnlyCollection<RoadEventDataDisplayInfo>(roadEventGroup.ToArray());
			this.SetOrToggleActiveRoadEvent(roadEventGroup.First());

			this.EnabledChanged += (s, e) => RefreshIconAndState();
		}

		private void RoadEventGroupControl_Click(object sender, EventArgs e)
		{
			var roadEvent = SetOrToggleActiveRoadEvent(this.ActiveRoadEvent);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);

			foreach (var icon in _icons.Select(kv => kv.Value))
			{
				icon.Item1.Dispose();
				icon.Item2.Dispose();
			}
		}

		protected virtual void OnRoadEventClicked(EventArgs<RoadEventDataDisplayInfo> e)
		{
			var handler = RoadEventClicked;
			if (handler != null)
				handler(this, e);
		}

		public ReadOnlyCollection<RoadEventDataDisplayInfo> RoadEventGroup { get; private set; }

		public RoadEventDataDisplayInfo ActiveRoadEvent { get; private set; }

		private void RefreshIconAndState()
		{
			bool enabled = this.Enabled;
			var roadEvent = this.ActiveRoadEvent;

			picIcon.Enabled = enabled;
			picState.Enabled = enabled;

			if (roadEvent == null)
				return;

			picIcon.Image = enabled ? _icons[roadEvent].Item1 : _icons[roadEvent].Item2;
			ttpMain.SetToolTip(picIcon, string.Format("{0} ({1})", roadEvent.RoadEventDataTemplate.Description, roadEvent.Hotkey));

			if (roadEvent.RoadEventDataTemplate.IsSnapshot)
			{
				picState.Image = enabled ? ImageResources.SnapshotOff : ImageResources.SnapshotOn;
				picState.BackColor = this.BackColor;
			}
			else
			{
				picState.Image = enabled ? ImageResources.ToggleOff : ImageResources.ToggleOn;

				if (!enabled)
					picState.BackColor = Color.LightGray;
				else if (roadEvent.CustomColor != null)
					picState.BackColor = roadEvent.CustomColor.Value;
				else
					picState.BackColor = GetSeverityColor(roadEvent.RoadEventDataTemplate.Severity);
			}
		}

		public RoadEventDataDisplayInfo SetOrToggleActiveRoadEvent(RoadEventDataDisplayInfo roadEvent)
		{
			if (roadEvent == null) throw new ArgumentNullException("roadEvent");
			if (!this.RoadEventGroup.Contains(roadEvent)) throw new ArgumentException("roadEvent must belong to the group (RoadEventGroupControl.RoadEventGroup) linked to this control.", "roadEvent");

			if (roadEvent == this.ActiveRoadEvent)
			{
				var roadEventsWithSameKey =
					this.RoadEventGroup
						.Where(r => r.Hotkey == roadEvent.Hotkey && r.RoadEventDataTemplate.Severity != roadEvent.RoadEventDataTemplate.Severity)
						.OrderBy(r => r.RoadEventDataTemplate.Severity);

				var nextRoadEvent = roadEventsWithSameKey.FirstOrDefault(r => r.RoadEventDataTemplate.Severity > roadEvent.RoadEventDataTemplate.Severity);

				if (nextRoadEvent == null)
					nextRoadEvent = roadEventsWithSameKey.FirstOrDefault();
				if (nextRoadEvent == null)
					nextRoadEvent = this.RoadEventGroup[0];

				roadEvent = nextRoadEvent;
			}

			this.ActiveRoadEvent = roadEvent;

			RefreshIconAndState();
			OnRoadEventClicked(new EventArgs<RoadEventDataDisplayInfo> { Value = roadEvent });

			return roadEvent;
		}

		private static Color GetSeverityColor(int severity)
		{
			if (severity < 0 || severity > 100) throw new ArgumentOutOfRangeException("severity", "severity must be between 0 and 100 inclusively.");

			int ratio = (severity * 255) / 50;
			int color2Ratio = 255 - ratio;
			Color colorStart;
			Color colorEnd;

			if (severity <= 50)
			{
				colorStart = Color.Green;
				colorEnd = Color.Yellow;
			}
			else
			{
				colorStart = Color.Yellow;
				colorEnd = Color.Red;
			}

			int newA = (byte) ((colorEnd.A * ratio + colorStart.A * color2Ratio) / 255);
			int newR = (byte) ((colorEnd.R * ratio + colorStart.R * color2Ratio) / 255);
			int newG = (byte) ((colorEnd.G * ratio + colorStart.G * color2Ratio) / 255);
			int newB = (byte) ((colorEnd.B * ratio + colorStart.B * color2Ratio) / 255);

			return Color.FromArgb(newA, newR, newG, newB);
		}
	}
}