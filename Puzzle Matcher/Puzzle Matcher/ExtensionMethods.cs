using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

//TODO Przemysleć przechowywanie zmiennych tymczasowych zamiast "object[]"

namespace Puzzle_Matcher
{
	public static class ExtensionMethods
	{
		public static string ImagePath { get; set; }

		public static List<Bitmap> ImageOut { get; } = new List<Bitmap>();

		public static void FindEdges()
		{
			var q1 = new Image<Bgr, byte>(ImagePath);
			var q2 = q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate().Erode();

			var w3 = FindContours(q2.Copy());

			var avg = CalculateAvreage(w3.Item1);

			var e4 = new VectorOfVectorOfPoint();

			for (var i = 0; i < w3.Item1.Size; i++) if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);

			var q5 = q1.Copy().MarkCountours(e4, new MCvScalar(255, 0, 0)).PutText("Puzzles find: " + e4.Size, new Point(200, 250), new MCvScalar(255, 255, 255));

			var boundRect = new List<Rectangle>();

			for (var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));
			var x = 0;

			var puzzels = new List<Image<Bgr, byte>>();

			foreach (var r in boundRect)
			{
				x++;
				var img = q1.Copy();
				img.ROI = r;
				puzzels.Add(img.Copy());
				CvInvoke.Rectangle(q1, r, new MCvScalar(250, 0, 250), 10);
				CvInvoke.PutText
				(
					q1
					, x.ToString()
					, new Point(r.X + r.Width / 2, r.Y + r.Height / 2)
					, FontFace.HersheySimplex
					, 8
					, new MCvScalar(255, 0, 255)
					, 10);
			}

			ImageOut.Add(q1.ToBitmap());
			ImageOut.Add(q5.ToBitmap());
		}

		#region ExtensionMethods

		/// <summary>
		///     Resize the image to the specified width and height.
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

		public static Image<Gray, byte> GaussBlur
			(this Image<Gray, byte> inImage, Size ksize = new Size(), double sigmaX = 5, double sigmaY = 5)
		{
			var outImage = inImage.Copy();
			CvInvoke.GaussianBlur(inImage, outImage, ksize, sigmaX, sigmaY);
			return outImage;
		}

		public static Image<Gray, byte> AdaptiveThreshold
		(
			this Image<Gray, byte> inImage
			, double maxVal = 250
			, AdaptiveThresholdType adaptiveThresholdType = AdaptiveThresholdType.MeanC
			, ThresholdType thresholdType = ThresholdType.BinaryInv
			, int blockSize = 39
			, double param1 = 4
		)
		{
			var outImage = inImage.Copy();
			CvInvoke.AdaptiveThreshold(GaussBlur(inImage), outImage, maxVal, adaptiveThresholdType, thresholdType, blockSize, param1);
			return outImage;
		}

		public static Image<Gray, byte> Dilate(this Image<Gray, byte> inImage, int iterations = 5)
		{
			var outImage = inImage.Copy();
			inImage._Dilate(iterations);
			return outImage;
		}

		public static Image<Gray, byte> Erode(this Image<Gray, byte> inImage, int iterations = 4)
		{
			var outImage = inImage.Copy();
			inImage._Erode(iterations);
			return outImage;
		}

		public static Tuple<VectorOfVectorOfPoint, Mat> FindContours(Image<Gray, byte> inImage, RetrType retrType = RetrType.External, ChainApproxMethod chainApproxMethod = ChainApproxMethod.ChainApproxSimple)
		{
			var contours = new VectorOfVectorOfPoint();
			var hierarchy = new Mat();

			CvInvoke.FindContours(inImage, contours, hierarchy, retrType, chainApproxMethod);

			return new Tuple<VectorOfVectorOfPoint, Mat>(contours, hierarchy);
		}

		public static double CalculateAvreage(VectorOfVectorOfPoint contours, double constant = 0.85)
		{
			double avg = 0;

			for (var i = 0; i < contours.Size; i++) avg += CvInvoke.ContourArea(contours[i]);

			avg /= contours.Size;
			avg *= constant;

			return avg;
		}

		public static Image<Bgr, byte> MarkCountours(this Image<Bgr, byte> inImage, VectorOfVectorOfPoint contours, MCvScalar color, int index = -1, int thickness = 10)
		{
			var outImage = inImage.Copy();
			CvInvoke.DrawContours(outImage, contours, index, color, thickness);
			return outImage;
		}

		public static Image<Bgr, byte> PutText(this Image<Bgr, byte> inImage, string text, Point where, MCvScalar color, FontFace fontFace = FontFace.HersheySimplex, int fontScale = 10, int thickness = 2)
		{
			var outImage = inImage.Copy();
			CvInvoke.PutText(outImage, text, where, fontFace, fontScale, color, thickness);
			return outImage;
		}

		#endregion ExtensionMethods
	}
}