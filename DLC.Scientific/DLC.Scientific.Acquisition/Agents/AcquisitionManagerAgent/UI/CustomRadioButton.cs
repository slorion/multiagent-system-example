using System;
using System.Drawing;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	public class CustomRadioButton
		: RadioButton
	{
		private string _textCustom = string.Empty;

		public Color DisabledColor { get; set; }

		public String TextCustom
		{
			get
			{
				return _textCustom;
			}
			set
			{
				_textCustom = value;

				// Padding is required to fix custom text display during OnPaint Permet un affichage correct du custom text lors du OnPaint
				base.Text = "".PadLeft(this._textCustom.Length + 6);
			}
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(this.TextCustom))
					base.Text = value;
				else
				{
					// Padding is required to fix custom text display during OnPaint Permet un affichage correct du custom text lors du OnPaint
					base.Text = "".PadLeft(_textCustom.Length + 6);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.Enabled)
			{
				base.OnPaint(e);

				using (var format = new StringFormat(StringFormat.GenericDefault))
				{
					format.Alignment = StringAlignment.Center;
					using (var brush = new SolidBrush(this.ForeColor))
					{
						e.Graphics.DrawString(this.TextCustom, this.Font, brush, this.ClientRectangle, format);
					}
				}
			}
			else
			{
				base.OnPaint(e);

				using (var format = new StringFormat(StringFormat.GenericDefault))
				{
					format.Alignment = StringAlignment.Center;
					using (var brush = new SolidBrush(this.DisabledColor))
					{
						e.Graphics.DrawString(this.TextCustom, this.Font, brush, this.ClientRectangle, format);
					}
				}
			}
		}
	}
}