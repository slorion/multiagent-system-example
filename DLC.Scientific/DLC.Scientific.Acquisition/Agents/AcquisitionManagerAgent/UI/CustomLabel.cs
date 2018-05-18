using System;
using System.Drawing;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	public class CustomLabel
		: Label
	{
		public Color DisabledColor { get; set; }

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.Enabled)
				base.OnPaint(e);
			else
			{
				using (var brush = new SolidBrush(this.DisabledColor))
				{
					e.Graphics.DrawString(this.Text, this.Font, brush, this.ClientRectangle);
				}
			}
		}
	}
}