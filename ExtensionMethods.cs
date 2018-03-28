using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;

namespace Puzzle_Matcher
{
	public static class ExtensionMethods


	{
        


        public static string ImagePath { get; set; }

        public static object ImageOut { get; set; }

        public static object ImageOut2 { get; set; }

        public static object ImageIn { get; set; }

        public static void findEdges()
        {
            Image<Bgr, Byte> My_Image = new Image<Bgr, Byte>(ImagePath);
            Image<Gray, Byte> gray = My_Image.Convert<Gray, Byte>();

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();

            CvInvoke.GaussianBlur(gray, gray, new Size(), 5, 5, BorderType.Default);

            Image<Gray, Byte> Imgout =gray;


             CvInvoke.AdaptiveThreshold(gray, Imgout, 250, Emgu.CV.CvEnum.AdaptiveThresholdType.MeanC, ThresholdType.BinaryInv, 39, 4);
            //CvInvoke.GaussianBlur(gray, gray, new Size(), 5, 5, BorderType.Default);
           


             Imgout._Dilate(5);
             Imgout._Erode(4);



            CvInvoke.FindContours(Imgout, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);           
            VectorOfVectorOfPoint corected = new VectorOfVectorOfPoint();       
            double max = 10;

           

            for (int i = 0; i < contours.Size; i++) 

            {

                if (CvInvoke.ContourArea(contours[i],false) > max)
                {

                    max = CvInvoke.ContourArea(contours[i], false);

                    corected.Push(contours[i]);

                }

            }

          
            CvInvoke.DrawContours(My_Image, contours, -1, new MCvScalar(255, 0, 0),10);
            //CvInvoke.PutText(My_Image, "puzzles find:" + corected.Size, new Point(200, 250), FontFace.HersheySimplex, 10, new MCvScalar(255, 0, 0),2);


            ImageOut2 = My_Image.ToBitmap();
            ImageOut = Imgout.ToBitmap();


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