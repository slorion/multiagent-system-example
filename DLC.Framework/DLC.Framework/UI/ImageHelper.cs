using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DLC.Framework.UI
{
	public static class ImageHelper
	{
		public static Image MakeGrayscale(Image source)
		{
			if (source == null) throw new ArgumentNullException("source");

			var newBitmap = new Bitmap(source.Width, source.Height);

			using (var g = Graphics.FromImage(newBitmap))
			{
				var grayScaleColorMatrix = new ColorMatrix(
					new float[][] {
						new float[] {.3f, .3f, .3f, 0, 0},
						new float[] {.59f, .59f, .59f, 0, 0},
						new float[] {.11f, .11f, .11f, 0, 0},
						new float[] {0, 0, 0, 1, 0},
						new float[] {0, 0, 0, 0, 1}
					});

				using (var attributes = new ImageAttributes())
				{
					attributes.SetColorMatrix(grayScaleColorMatrix);
					g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
				}
			}

			return newBitmap;
		}
	}
}