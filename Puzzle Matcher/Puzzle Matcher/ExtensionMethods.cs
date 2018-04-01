using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Puzzle_Matcher
{
	public static class ExtensionMethods
	{
		public static string ImagePath { get; set; }

		public static object ImageOut { get; set; }

		public static void MyFirstMethod()
		{
			Image<Bgr, Byte> My_Image = new Image<Bgr, Byte>(ImagePath);
			Image<Gray, Byte> ibw = My_Image.Convert<Gray, Byte>();

			Image<Gray, Byte> ibw1 = ibw.Canny(100, 200);

			ibw1._Dilate(1);
			ibw1._Erode(1);

			ImageOut = ibw1.ToBitmap();
		}

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">Image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Bitmap image, int width, int height)
		{
			var destImage = new Bitmap(width, height);
			var destRectangle = new Rectangle(0, 0, width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			var graphics = Graphics.FromImage(destImage);
			graphics.CompositingMode = CompositingMode.SourceCopy;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			var wrapMode = new ImageAttributes();
			wrapMode.SetWrapMode(WrapMode.TileFlipXY);
			graphics.DrawImage(image, destRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

			return destImage;
		}

	}
}