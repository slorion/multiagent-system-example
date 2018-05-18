using System;
using System.Drawing;
using System.Windows.Forms;

namespace DLC.Framework.UI.Forms.Controls
{
	public class ActivablePictureBox
		: PictureBox
	{
		private Image _activeImage;
		private readonly Lazy<Image> _inactiveImage;

		public ActivablePictureBox()
		{
			_inactiveImage = new Lazy<Image>(() => ImageHelper.MakeGrayscale(this.Image));

			this.EnabledChanged +=
				(s, e) => {
					if (this.Image == null)
						return;

					if (_activeImage == null)
						_activeImage = this.Image;

					this.Image = this.Enabled ? _activeImage : _inactiveImage.Value;
				};
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_inactiveImage != null && _inactiveImage.IsValueCreated && _inactiveImage.Value != null)
				_inactiveImage.Value.Dispose();
		}
	}
}