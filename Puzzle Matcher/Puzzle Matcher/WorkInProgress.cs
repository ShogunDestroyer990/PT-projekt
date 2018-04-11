using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using Emgu.CV.Features2D;
using Emgu.CV.Flann;
using System.Linq;

namespace Puzzle_Matcher
{
	public partial class WorkInProgress : Form
	{
		public WorkInProgress()
		{
			InitializeComponent();

			Worker.RunWorkerAsync();
		}

		private void Progress(int progresPercent, string description = null)
		{
			if(description != null) Description.Text = description;
			Worker.ReportProgress(progresPercent);
		}

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //TODO: Calls stack with pattern
            //ExtensionMethods.Method();
            //Invoke(new Action(delegate { Progress(int, string); }));

            Invoke(new Action(delegate { Progress(0, "Wczytywanie obrazka..."); }));
            var q1 = new Image<Bgr, byte>(ExtensionMethods.ImagePath);


            Invoke(new Action(delegate { Progress(10, "Przygotowywanie obrazka do obróbki..."); }));
            var q2 = q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate(8).Erode(); //dyalatacja sprawuje się o wiele lepiej w znajdywaniu puzzli gdy jest większa


            Invoke(new Action(delegate { Progress(20, "Znajdowanie wszystkich konturów..."); }));
            var w3 = ExtensionMethods.FindContours(q2.Copy());


            Invoke(new Action(delegate { Progress(30, "Wybieranie puzzli..."); }));
            var avg = ExtensionMethods.CalculateAvreage(w3.Item1);
            var e4 = new VectorOfVectorOfPoint();
            for (var i = 0; i < w3.Item1.Size; i++) if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);


            Invoke(new Action(delegate { Progress(40, "Zaznaczanie puzzli..."); }));
            var q5 = q1.Copy().MarkCountours(e4, new MCvScalar(255, 0, 0)).PutText("Puzzles find: " + e4.Size, new Point(200, 250), new MCvScalar(255, 255, 255));



            Invoke(new Action(delegate { Progress(50, "Robienie innych rzeczy..."); }));
            var boundRect = new List<Rectangle>();

            for (var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));
            var x = 0;

            var puzzels = new List<Image<Bgr, byte>>();

            foreach (var r in boundRect)
            {
                x++;
                if (x == boundRect.Count / 3) Invoke(new Action(delegate { Progress(60); }));
                if (x == (2 * boundRect.Count) / 3) Invoke(new Action(delegate { Progress(70); }));
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

            Invoke(new Action(delegate { Progress(80, "Zapisywanie"); }));


            foreach (Image<Bgr, byte> puzzel in puzzels)
            {
                Image<Gray, Byte> gray = puzzel.Convert<Gray, Byte>();
                Mat hierarchy = new Mat();
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Image<Gray, Byte> Imgout = gray;

                CvInvoke.AdaptiveThreshold(gray, Imgout, 250, Emgu.CV.CvEnum.AdaptiveThresholdType.MeanC, ThresholdType.BinaryInv, 39, 4);
                Imgout._Dilate(5);
               // Imgout._Erode(3);

                CvInvoke.FindContours(Imgout, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                VectorOfVectorOfPoint Max = new VectorOfVectorOfPoint();
                Max.Push(contours[0]);

                for (int i = 0; i < contours.Size; i++)
                {
                    if (CvInvoke.ContourArea(contours[i], false) > CvInvoke.ContourArea(Max[0], false))
                    {
                        Max.Clear();
                        Max.Push(contours[i]);
                      
                    }
                }

                Image<Bgr, Byte> puzzelcopy = puzzel.Clone();

                CvInvoke.FillPoly(puzzelcopy, Max, new MCvScalar(0, 0, 255)); //zamalowuje puzzle\
              
                   Bgr white = new Bgr(0, 0, 255);
                Bgr transparent = new Bgr(Color.Transparent);
                
                Bitmap copy = new Bitmap(puzzelcopy.Bitmap);

                for (int j = 1; j < puzzelcopy.Cols; j++)
                {
                    for (int i = 1; i < puzzelcopy.Rows; i++)
                    {
                        if(!puzzelcopy[i,j].Equals(white))
                       // if (!copy.GetPixel(i,j).Equals(Color.White))
                        {
                            puzzel[i, j] = transparent;
                        }

                       
                    }
                }


                ExtensionMethods.ImageOut.Add(puzzel.ToBitmap());
            }
            //	ExtensionMethods.ImageOut.Add(q5.ToBitmap());

            //rozpoznawanie -> keypoints 
            //tak nawaliłem trochę kodu

            /*     var k1 = q1.Copy(); //dla wybranego obrazka
                 SURF surf = new SURF(600); //surf 
                 var keypoints = new VectorOfKeyPoint(); //keyponty
                 UMat kdesc = new UMat();
                 surf.DetectAndCompute(q1, null, keypoints, kdesc, false);

                 Features2DToolbox.DrawKeypoints(q1, keypoints, k1, new Bgr(0, 255, 0));
                 //jeśli chcemy uzywac keypointów logiczne jest że musimy miec orginał...
                 //więc
                 // var orginal = new Image<Bgr, byte>(ExtensionMethods.ImagePath);
                 //żartuje , nie chce mi się dodawać na razie metody dodawania drugiego obrazka więc
                 var orginal = new Image<Bgr, byte>("path"); //tu powinna być ścieżka

                 var copyOrginal = orginal.Copy();
                 var orginalKeypoints = new VectorOfKeyPoint(); //keyponty
                 UMat odesc = new UMat();
                 surf.DetectAndCompute(copyOrginal, null, orginalKeypoints, odesc, false);

                 Features2DToolbox.DrawKeypoints(orginal, orginalKeypoints, copyOrginal, new Bgr(0, 255, 0));



                 //dopasowywanie
                 BFMatcher matcher = new BFMatcher(DistanceType.L2);
                 VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();
                 matcher.Add(odesc);
                 matcher.KnnMatch(kdesc, matches, 2, null);


                 Mat result = new Mat();

                 Features2DToolbox.DrawMatches(copyOrginal, orginalKeypoints, k1, keypoints, matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), null);


                 Image<Bgr, Byte> res = new Image<Bgr, Byte>(result.Bitmap);






                 foreach (Image<Bgr, byte> puzzel in puzzels)
                 {
                     var puzzelpoints = new VectorOfKeyPoint(); //keyponty
                     UMat pdesc = new UMat();
                     surf.DetectAndCompute(puzzel, null, puzzelpoints, pdesc, false);

                     VectorOfVectorOfDMatch puzzelmatches = new VectorOfVectorOfDMatch();
                     matcher.KnnMatch(pdesc, puzzelmatches, 3, null);
                     Mat Result = new Mat();
                     Features2DToolbox.DrawMatches(copyOrginal, orginalKeypoints, puzzel, puzzelpoints, puzzelmatches, Result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), null);
                     Image<Bgr, Byte> MachedPuzzel = new Image<Bgr, Byte>(Result.Bitmap);

                      ExtensionMethods.ImageOut.Add(MachedPuzzel.ToBitmap());
                     //  Bgr color = puzzel[0, 0]; //czytanie koloru (przydatne do układania kolorami)
                     //System.Console.Out.Write(color.ToString());
                 }


                 ExtensionMethods.ImageOut.Add(res.ToBitmap());
                 ExtensionMethods.ImageOut.Add(k1.ToBitmap());
                 ExtensionMethods.ImageOut.Add(copyOrginal.ToBitmap());*/
            ExtensionMethods.ImageOut.Add(q1.ToBitmap());


            Invoke(new Action(delegate { Progress(100, "DONE"); }));

            Thread.Sleep(100);
        }

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			new Thread(() => { Application.Run(new ResultWindow()); }).Start();

			Close();
		}
	}
}