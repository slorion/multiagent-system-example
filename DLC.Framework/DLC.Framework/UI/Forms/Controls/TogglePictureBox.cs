using System;
using System.Drawing;

namespace DLC.Framework.UI.Forms.Controls
{
	public partial class TogglePictureBox
		: ActivablePictureBox
	{
		private Image _pushedImage;
		private Image _unPushedImage;

		public event EventHandler PushOrUnPush;

		public TogglePictureBox()
		{
			InitializeComponent();

			this.MouseClick +=
				(s, e) =>
				{
					if (!this.Enabled)
						return;

					if (this.PushedImage == null && this.UnPushedImage == null)
						return;

					if (this.Toggle)
						this.Image = this.UnPushedImage;
					else
						this.Image = this.PushedImage;

					this.Toggle = !this.Toggle;

					OnPushOrUnPush(EventArgs.Empty);
				};
		}

		public bool Toggle { get; set; }

		protected virtual void OnPushOrUnPush(EventArgs e)
		{
			var handler = this.PushOrUnPush;
			if (handler != null)
				handler(this, e);
		}

		public void Inverse(bool raiseEvent = false)
		{
			if (!this.Enabled)
				return;

			this.Toggle = !this.Toggle;

			Init();

			if (raiseEvent)
				OnPushOrUnPush(EventArgs.Empty);
		}

		public void Push(bool raiseEvent = false)
		{
			if (!this.Enabled)
				return;

			this.Toggle = true;

			Init();

			if (raiseEvent)
				OnPushOrUnPush(EventArgs.Empty);
		}

		public void UnPush(bool raiseEvent = false)
		{
			if (!this.Enabled)
				return;

			this.Toggle = false;

			Init();

			if (raiseEvent)
				OnPushOrUnPush(EventArgs.Empty);
		}

		public Image PushedImage
		{
			get { return _pushedImage; }
			set
			{
				_pushedImage = value;
				Init();
			}
		}

		public Image UnPushedImage
		{
			get { return _unPushedImage; }
			set
			{
				_unPushedImage = value;
				Init();
			}
		}

		private void Init()
		{
			if (this.Toggle)
			{
				if (this.PushedImage != null)
					this.Image = this.PushedImage;
			}
			else
			{
				if (this.UnPushedImage != null)
					this.Image = this.UnPushedImage;
			}
		}
	}
}