using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Puzzle_Matcher.Helpers
{
	public static class ExtensionMethods
	{
		public static string ImagePath { get; set; }

		public static string OrginalImagePath { get; set; }

		public static List<Bitmap> ImageOut { get; } = new List<Bitmap>();

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

		public static Image<Gray, byte> GaussBlur(this Image<Gray, byte> inImage, Size ksize = new Size(), double sigmaX = 5, double sigmaY = 5)
		{
			var outImage = inImage.Copy();
			CvInvoke.GaussianBlur(inImage, outImage, ksize, sigmaX, sigmaY);
			return outImage;
		}

		public static Image<Gray, byte> AdaptiveThreshold(this Image<Gray, byte> inImage, double maxVal = 250, AdaptiveThresholdType adaptiveThresholdType = AdaptiveThresholdType.MeanC, ThresholdType thresholdType = ThresholdType.BinaryInv, int blockSize = 39, double param1 = 4)
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

		public static Image<Bgr, byte> PutText(this Image<Bgr, byte> inImage, string text, Point where, MCvScalar color, FontFace fontFace = FontFace.HersheySimplex, int fontScale = 10, int thickness = 2)
		{
			var outImage = inImage.Copy();
			CvInvoke.PutText(outImage, text, where, fontFace, fontScale, color, thickness);
			return outImage;
		}

		public static Image<Bgr, byte> Rectangle(this Image<Bgr, byte> inImage, Rectangle r, MCvScalar color, int thickness = 10, LineType lt = LineType.EightConnected, int shift = 0)
		{
			var outImage = inImage.Copy();
			CvInvoke.Rectangle(outImage, r, color, thickness, lt, shift);
			return outImage;
		}

		public static void DrawSymbol(this Image img, string symbol, Brush backgroundBrush, Font font, Brush textBrush)
		{
			using (var g = Graphics.FromImage(img))
			{
				g.FillRectangle(backgroundBrush, 0, 0, img.Width, img.Height);
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.DrawString(symbol, font, textBrush, new Point(img.Width / 2, img.Height / 2), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
				g.Flush();
			}
		}

		public static Image<Bgr, byte> GenerateFinalpicture(int puzzelX, int puzzelY, int[] resultTab, List<Image<Bgr, byte>> puzzels)
		{
			var puzzelCounter = puzzelX * puzzelY;
			var finalsumX = 0;
			var finalsumY = 0;
			var mumIsFinalPicDone = false;
			var picId = 0;

			var puzzelXcounter = 0;
			var avragepuzzelSize = 0;

			foreach (var puzzel in puzzels)
			{
				avragepuzzelSize += puzzel.Width;
				avragepuzzelSize += puzzel.Height;
			}

			avragepuzzelSize /= (puzzelCounter * 2);

			var finalPic = new Image<Bgr, byte>((avragepuzzelSize * puzzelX), (avragepuzzelSize * puzzelY));
			var counterino = 0;

			while (mumIsFinalPicDone == false)
			{
				foreach (var puzzel in puzzels)
				{
					if (picId == resultTab[counterino])
					{
						if (puzzelX == puzzelXcounter)
						{
							puzzelXcounter = 0;
							finalsumX = 0;
							finalsumY += avragepuzzelSize;
						}

						finalPic.ROI = new Rectangle(finalsumX, finalsumY, avragepuzzelSize, avragepuzzelSize);

						var resizedImage = puzzel.Resize(avragepuzzelSize, avragepuzzelSize, Inter.Linear);

						resizedImage.CopyTo(finalPic);

						finalPic.ROI = System.Drawing.Rectangle.Empty;
						finalsumX += avragepuzzelSize;
					}
					picId++;
				}

				if ((counterino + 1) == puzzelCounter)
				{
					mumIsFinalPicDone = true;
				}
				else
				{
					picId = 0;
					counterino++;
					puzzelXcounter++;
				}
			}
			return finalPic;
		}

		private static int[] PlacePuzzlesHelper(int e1, int e2, int[] ee1, int[] ee2)
		{
			var puzzelOrder = new int[(e1 * e2)];
			var blocks = new List<int[]>();

			for (var i = 0; i < (e1 * e2); i++)
			{
				if (i % e1 != 0) continue;
				var block = new int[e1];
				for (var j = 0; j < e1; j++)
				{
					block[j] = ee1[i + j];
				}
				blocks.Add(block);
			}

			var counter = 0;
			foreach (var block in blocks)
			{
				foreach (var tX in ee2)
				{
					foreach (var b in block)
					{
						if (b != tX) continue;
						puzzelOrder[counter] = b;
						counter++;
					}
				}
			}

			return puzzelOrder;
		}

		private static int[] SortedArray(double[] a)
		{
			var copyOfA = (double[])a.Clone();
			var tab = new int[a.Length];
			Array.Sort(a);

			for (var i = 0; i < a.Length; i++)
			{
				for (var j = 0; j < a.Length; j++)
				{
					if (a[i] == copyOfA[j]) tab[i] = j;
				}
			}

			return tab;
		}

		public static int[] PlacePuzzels(int hort, int vert, double[] avgX, double[] avgY)
		{
			var tabX = SortedArray(avgX);
			var tabY = SortedArray(avgY);

			if (hort >= vert) return PlacePuzzlesHelper(hort, vert, tabY, tabX);

			var puzzelOrder = PlacePuzzlesHelper(vert, hort, tabX, tabY);

			//zamiana na -> \|/ kolejność
			var puzzelOrdercopy = (int[])puzzelOrder.Clone();
			var counter = 0;
			var bigcounter = 1;
			for (var i = 0; i < puzzelOrder.Length; i++)
			{
				puzzelOrder[i] = puzzelOrdercopy[counter];
				counter += vert;
				if (counter < puzzelOrder.Length) continue;
				counter = bigcounter;
				bigcounter++;
			}

			return puzzelOrder;
		}

		public static VectorOfVectorOfDMatch KnnMatch(this BFMatcher matcher, UMat pdesc, int i, IInputArray o = null)
		{
			var puzzelmatches = new VectorOfVectorOfDMatch();
			matcher.KnnMatch(pdesc, puzzelmatches, i, o);
			return puzzelmatches;
		}

		public static Tuple<UMat, VectorOfKeyPoint> DetectAndCompute(SURF surf, Image<Bgr, byte> image, bool b, IInputArray inputArray = null)
		{
			var keypoints = new VectorOfKeyPoint();
			var desc = new UMat();
			surf.DetectAndCompute(image, inputArray, keypoints, desc, b);
			return new Tuple<UMat, VectorOfKeyPoint>(desc, keypoints);
		}

		public static UMat DetectAndCompute(this SURF surf, Image<Bgr, byte> image)
		{
			return DetectAndCompute(surf, image, false).Item1;
		}

		#endregion ExtensionMethods
	}
}