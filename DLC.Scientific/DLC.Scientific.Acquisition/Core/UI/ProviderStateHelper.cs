using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DLC.Scientific.Acquisition.Core.UI
{
	public static class ProviderStateHelper
	{
		public static Tuple<Color, string> GetStateDescription(ProviderState state)
		{
			Color color;
			string text;

			switch (state)
			{
				case ProviderState.Created:
					color = Color.Gray;
					text = "Not connected";
					break;
				case ProviderState.Initializing:
					color = Color.SandyBrown;
					text = "Initializing...";
					break;
				case ProviderState.Initialized:
					color = Color.Orange;
					text = "Initialized";
					break;
				case ProviderState.Starting:
					color = Color.SteelBlue;
					text = "Starting...";
					break;
				case ProviderState.Started:
					color = Color.Navy;
					text = "Started";
					break;
				case ProviderState.InitializingRecord:
					color = Color.Khaki;
					text = "Initializing record step...";
					break;
				case ProviderState.InitializedRecord:
					color = Color.Gold;
					text = "Recording step prepared";
					break;
				case ProviderState.StartingRecord:
					color = Color.LightGreen;
					text = "Starting record step...";
					break;
				case ProviderState.StartedRecord:
					color = Color.LimeGreen;
					text = "Recording step started";
					break;
				case ProviderState.StoppingRecord:
					color = Color.LightGreen;
					text = "Stopping recording step...";
					break;
				case ProviderState.Stopping:
					color = Color.SteelBlue;
					text = "Stopping...";
					break;
				case ProviderState.Uninitializing:
					color = Color.SandyBrown;
					text = "Uninitializing...";
					break;
				case ProviderState.UninitializingRecord:
					color = Color.Khaki;
					text = "Uninitializing recording step...";
					break;
				case ProviderState.Calibrating:
					color = Color.Fuchsia;
					text = "Calibrating...";
					break;
				case ProviderState.Failed:
					color = Color.Red;
					text = "Failed";
					break;
				case ProviderState.Disposed:
					color = Color.Black;
					text = "Disposed";
					break;

				default:
					color = Color.White;
					text = "?????";
					break;
			}

			return Tuple.Create(color, text);
		}

		public static Bitmap GetStateImage(int size, ProviderState state)
		{
			var stateColor = GetStateDescription(state).Item1;
			return GetStateImage(size, stateColor);
		}

		public static Bitmap GetStateImage(int size, Color innerColor)
		{
			return GetStateImage(size, innerColor, Color.White);
		}

		public static Bitmap GetStateImage(int size, Color primary, Color secondary)
		{
			if (size < 1) throw new ArgumentOutOfRangeException("size", size, "size must be greater than 0.");

			var stateImage = new Bitmap(size, size);

			using (var g = Graphics.FromImage(stateImage))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;

				using (var gp = new GraphicsPath())
				{
					gp.AddEllipse(0, 0, size, size);

					using (var pgb = new PathGradientBrush(gp))
					{
						var blend = new ColorBlend();

						if (size > 64)
						{
							blend.Positions = new[] { 0f, 0.1f, 0.3f, .4f, .7f, 1f };
							blend.Colors = new[] { Color.Transparent, Color.Black, Color.Silver, Color.Black, secondary, primary };
						}
						else
						{
							blend.Positions = new[] { 0f, .4f, .7f, 1f };
							blend.Colors = new[] { Color.Transparent, Color.Black, secondary, primary };
						}

						pgb.InterpolationColors = blend;
						pgb.FocusScales = new PointF(0.75f, 0.75f);

						g.FillPath(pgb, gp);
					}
				}
			}

			return stateImage;
		}
	}
}