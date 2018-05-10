using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

namespace Puzzle_Matcher
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

		public static Image<Bgr, byte> Rectangle(this Image<Bgr, byte> inImage, Rectangle r, MCvScalar color, int thickness = 10, LineType lt = LineType.EightConnected, int shift = 0)
		{
			var outImage = inImage.Copy();
			CvInvoke.Rectangle(outImage, r, color, thickness, lt, shift);
			return outImage;
		}

		public static Image<Bgr, byte> FillPoly(this Image<Bgr, byte> inImage, VectorOfVectorOfPoint points, MCvScalar color, LineType lt = LineType.EightConnected, int shift = 0, Point offset = default(Point))
		{
			var outImage = inImage.Copy();
			CvInvoke.FillPoly(outImage, points, color, lt, shift, offset);
			return outImage;
		}

		public static UMat DetectAndCompute(this SURF s, Image<Bgr, byte> inImage, VectorOfKeyPoint keyPoints, IInputArray iia = null, bool useProvidedKeyPoints = true)
		{
			var descriptors = new UMat();
			s.DetectAndCompute(inImage, iia, keyPoints, descriptors, useProvidedKeyPoints);
			return descriptors;
		}

		public static Tuple<VectorOfKeyPoint, UMat> DetectAndCompute(this SURF s, Image<Bgr, byte> inImage, IInputArray iia = null, bool useProvidedKeyPoints = false)
		{
			var descriptors = new UMat();
			var keyPoints = new VectorOfKeyPoint();
			s.DetectAndCompute(inImage, iia, keyPoints, descriptors, useProvidedKeyPoints);
			return new Tuple<VectorOfKeyPoint, UMat>(keyPoints, descriptors);
		}

		public static Image DrawSymbol(this Image img, string symbol, Brush backgroundBrush, Font font, Brush textBrush)
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
			return img;
		}

		public static Image<Bgr, byte> generateFinalpicture(int puzzelX, int puzzelY, int[] resultTab, List<Image<Bgr, byte>> puzzels)
		{// puzzelX i Y przedstawiajają jak ma być ułożony obraz
			int puzzelCounter = puzzelX * puzzelY;
			int finalsumX = 0;
			int finalsumY = 0;
			bool mumIsFinalPicDone = false;
			int picId = 0;

			int puzzelXcounter = 0;
			//nie chce mi się......
			int avragepuzzelSize = 0;
			foreach (Image<Bgr, byte> puzzel in puzzels)
			{

				avragepuzzelSize += puzzel.Width;
				avragepuzzelSize += puzzel.Height;
			}

			avragepuzzelSize /= (puzzelCounter * 2);



			var finalPic = new Image<Bgr, byte>((avragepuzzelSize * puzzelX), (avragepuzzelSize * puzzelY));
			int counterino = 0;


			while (mumIsFinalPicDone == false)
			{
				foreach (Image<Bgr, byte> puzzel in puzzels)
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

						Image<Bgr, byte> resizedImage = puzzel.Resize(avragepuzzelSize, avragepuzzelSize, Inter.Linear);

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

		public static int[] PlacePuzzels(int hort, int vert, double[] avgX, double[] avgY) //układanie puzzli po średnich punktach (keypoints)
		{
			//mały init :D
			double[] copyavgX = (double[])avgX.Clone();
			double[] copyavgY = (double[])avgY.Clone();

			int[] tabX = new int[avgX.Length];
			int[] tabY = new int[avgY.Length];

			Array.Sort(avgX);

			for (var i = 0; i < avgX.Length; i++)
			{
				for (var j = 0; j < avgX.Length; j++)
				{


					if (avgX[i] == copyavgX[j])
					{
						tabX[i] = j;
					}
				}
			}

			Array.Sort(avgY);

			for (int i = 0; i < avgY.Length; i++)
			{
				for (int j = 0; j < avgY.Length; j++)
				{
					if (avgY[i] == copyavgY[j])
					{
						tabY[i] = j;
					}
				}
			}


			if (hort >= vert)
			{
				int[] puzzelOrder = new int[(hort * vert)];
				List<int[]> blocks = new List<int[]>();

				for (int i = 0; i < (hort * vert); i++)
				{
					if (i % hort == 0)
					{
						int[] block = new int[hort];
						for (int j = 0; j < hort; j++)
						{
							block[j] = tabY[i + j];
						}
						blocks.Add(block);
					}
				}


				int counter = 0;
				foreach (int[] block in blocks)
				{
					for (int j = 0; j < tabX.Length; j++)
					{
						for (int i = 0; i < block.Length; i++)
						{
							if (block[i] == tabX[j])
							{
								puzzelOrder[counter] = block[i];
								counter++;
							}
						}
					}

				}

				return puzzelOrder;


			}
			else
			{

				int[] puzzelOrder = new int[(hort * vert)];
				List<int[]> blocks = new List<int[]>();

				for (int i = 0; i < (hort * vert); i++)
				{
					if (i % vert == 0)
					{
						int[] block = new int[vert];
						for (int j = 0; j < vert; j++)
						{
							block[j] = tabX[i + j];
						}
						blocks.Add(block);
					}
				}

				int counter = 0;
				foreach (int[] block in blocks)
				{
					for (int j = 0; j < tabY.Length; j++)
					{
						for (int i = 0; i < block.Length; i++)
						{
							if (block[i] == tabY[j])
							{
								puzzelOrder[counter] = block[i];
								counter++;
							}
						}
					}

				}

				//zamiana na -> \|/ kolejność
				int[] puzzelOrdercopy = (int[])puzzelOrder.Clone();
				counter = 0;
				int bigcounter = 1;
				for (int i = 0; i < puzzelOrder.Length; i++)
				{

					puzzelOrder[i] = puzzelOrdercopy[counter];
					counter += vert;
					if (counter >= puzzelOrder.Length)
					{
						counter = bigcounter;
						bigcounter++;
					}
				}

				return puzzelOrder;
			}


		}

		public static int[] assumePuzzelConfiguration(double[] avgPuzellXPoints, double[] avgPuzellYPoints, int puzzelCounter)
		{
			//dużo pętel
			//nie chce mi sie
			//ale działa
			double akumX = 0;
			double akumY = 0;

			int[] options = new int[puzzelCounter]; //tablica zawsze będzie za duża :\
			int cunt = 0;

			for (int i = 1; i <= puzzelCounter; i++)
			{
				if (puzzelCounter % i == 0)
				{
					options[cunt] = i;
					cunt++;
				}
			}


			for (int i = 0; i < puzzelCounter; i++)
			{
				akumX += avgPuzellXPoints[i];
				akumY += avgPuzellYPoints[i];


			} //liczenie średniej

			akumX /= puzzelCounter;
			akumY /= puzzelCounter;
			int scoreX = 0;
			int scoreY = 0;
			cunt = 0;
			for (int i = 0; i < puzzelCounter; i++)
			{
				if ((akumX * 1.02) > avgPuzellXPoints[i] && (akumX * 0.98) < avgPuzellXPoints[i])  //ahh nie mam pomysłów
				{
					scoreX++;
				}

				if ((akumY * 1.02) > avgPuzellYPoints[i] && (akumY * 0.98) < avgPuzellYPoints[i])
				{
					scoreY++;
				}

				if (options[i] != 0)
				{
					cunt++;
				}
			}//liczenie wyniku

			int[] difrenceX = new int[cunt];
			int[] difrenceY = new int[cunt];

			bool waitfordecisionX = false;

			bool waitfordecisionY = false;

			int minimumX = puzzelCounter;
			int minimumY = puzzelCounter;

			int[] decision = new int[2]; //0 -> X  1->Y


			for (int i = 0; i < cunt; i++)
			{
				difrenceX[i] = Math.Abs(options[i] - scoreX);
				difrenceY[i] = Math.Abs(options[i] - scoreY);

				if (difrenceX[i] == minimumX)
				{
					waitfordecisionX = true;
				}

				if (difrenceY[i] == minimumY)
				{
					waitfordecisionY = true;
				}

				if (difrenceX[i] < minimumX)
				{
					minimumX = difrenceX[i];
					waitfordecisionX = false;
					decision[0] = options[i];
				}

				if (difrenceY[i] < minimumY)
				{
					minimumY = difrenceY[i];
					waitfordecisionY = false;
					decision[1] = options[i];
				}


				// CvInvoke.PutText(k1, difrenceX[i].ToString(), new Point(100, 1800 + (100 * i)), FontFace.HersheySimplex, 4, new MCvScalar(255, 0, 255), 4);
				// CvInvoke.PutText(k1, difrenceY[i].ToString(), new Point(1500, 1800 + (100 * i)), FontFace.HersheySimplex, 4, new MCvScalar(255, 0, 255), 4);

			}//im bliżej zera tym lepiej zliczanie różnicy

			/* */

			if (waitfordecisionX == true)
			{
				for (int i = 0; i < cunt; i++)
				{
					if (decision[1] == options[i])
					{
						decision[0] = options[cunt - 1 - i];
					}
				}

			}

			if (waitfordecisionY == true)
			{
				for (int i = 0; i < cunt; i++)
				{
					if (decision[1] == options[i])
					{
						decision[0] = options[cunt - 1 - i];
					}
				}

			}
			//jeśli się powtarzazły to wybieramy odwrotność
			//to pewnie nie będzie działać za często
			//przyda się opcja w menu żeby urzytkownik wybrał czy chce tego używać czy nie

			return decision;
		}


		#endregion ExtensionMethods
	}
}