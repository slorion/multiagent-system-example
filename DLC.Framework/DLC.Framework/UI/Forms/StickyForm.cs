using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DLC.Framework.UI.Forms
{
	/// <summary>
	/// Adds these features to the standard Form :
	///		- Sticky window behavior (IsSticky)
	///		- Window position can be saved on close (IsWindowPositionSavedOnClose)
	///		- Window size can be saved on close (IsWindowSizeSavedOnClose)
	///		- Window state can be saved on close (IsWindowStateSavedOnClose)
	/// </summary>
	public class StickyForm
		: Form
	{
		private StickyWindow _sticky;
		private bool _isSticky;

		private FormProperties _winInfos = null;

		public StickyForm()
			: base()
		{
			InitializeComponent();

			// valeurs par defaut
			this.IsWindowPositionSavedOnClose = false;
			this.IsWindowSizeSavedOnClose = false;
			this.IsWindowStateSavedOnClose = true;
			this.IsSticky = false;
			this.StickGap = 20;
		}

		[DefaultValue(false)]
		[Description("Save window position on close")]
		public bool IsWindowPositionSavedOnClose { get; set; }

		[DefaultValue(false)]
		[Description("Save window size on close")]
		public bool IsWindowSizeSavedOnClose { get; set; }

		[DefaultValue(true)]
		[Description("Save window state on close")]
		public bool IsWindowStateSavedOnClose { get; set; }

		[DefaultValue(false)]
		[Description("Sticky window behavior")]
		public bool IsSticky
		{
			get { return _isSticky; }
			set
			{
				if (value && _sticky == null)
				{
					_sticky = new StickyWindow(this);
					_sticky.StickOnMove = value;
					_sticky.StickOnResize = value;
					_sticky.StickToOther = value;
					_sticky.StickToScreen = value;
					_isSticky = true;
				}
				else if (!value && _sticky != null)
				{
					_sticky = null;
				}
			}
		}

		[DefaultValue(20)]
		[Description("Sticky window gap (in pixels)")]
		public int StickGap
		{
			get
			{
				if (_sticky != null)
					return _sticky.StickGap;
				else
					return 0;
			}
			set
			{
				if (_sticky != null)
					_sticky.StickGap = value;
			}
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			//
			// BaseForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(465, 334);
			this.Name = "BaseForm";
			this.Text = "BaseForm";
			this.Shown += new System.EventHandler(this.BaseFormShown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BaseFormFormClosing);
			this.ResumeLayout(false);
		}

		private void BaseFormShown(object sender, EventArgs e)
		{
			if (!this.DesignMode)
			{
				_winInfos = new FormProperties(this.Name);

				if (this.IsWindowStateSavedOnClose && !String.IsNullOrEmpty(_winInfos.State))
				{
					this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), _winInfos.State);
				}
				if (this.IsWindowPositionSavedOnClose)
				{
					if (_winInfos.Left >= 0)
						this.Left = _winInfos.Left;
					if (_winInfos.Top >= 0)
						this.Top = _winInfos.Top;
				}

				if (this.IsWindowSizeSavedOnClose)
				{
					if (_winInfos.Width > 50)
						this.Width = _winInfos.Width;
					if (_winInfos.Height > 50)
						this.Height = _winInfos.Height;

				}
				if (_sticky != null)
				{
					_sticky.StickOnMove = true;
					_sticky.StickOnResize = true;
					_sticky.StickToOther = true;
					_sticky.StickToScreen = true;
				}
			}
		}

		private void BaseFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.DesignMode)
			{
				if (this.IsWindowStateSavedOnClose)
				{
					_winInfos.State = this.WindowState.ToString();
					_winInfos.SaveState();
				}

				if (this.WindowState == FormWindowState.Normal)
				{
					if (_sticky != null)
					{
						_sticky.StickOnMove = false;
						_sticky.StickOnResize = false;
						_sticky.StickToOther = false;
						_sticky.StickToScreen = false;
					}

					if (_winInfos != null)
					{
						if (this.IsWindowPositionSavedOnClose)
						{
							_winInfos.Top = this.Top;
							_winInfos.Left = this.Left;
						}

						if (this.IsWindowSizeSavedOnClose)
						{
							_winInfos.Height = this.Height;
							_winInfos.Width = this.Width;
						}

						if ( this.IsWindowPositionSavedOnClose || this.IsWindowSizeSavedOnClose)
							_winInfos.Save();
					}
				}
			}
		}
	}
}