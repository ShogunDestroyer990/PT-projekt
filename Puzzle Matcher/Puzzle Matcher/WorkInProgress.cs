using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

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
			if (description != null) Description.Text = description;
			Worker.ReportProgress(progresPercent);
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			Invoke(new Action(delegate { Progress(0, "Wczytywanie obrazka..."); }));
			var q1 = new Image<Bgr, byte>(ExtensionMethods.ImagePath);

			Invoke(new Action(delegate { Progress(10, "Przygotowywanie obrazka do obróbki..."); }));

			//dyalatacja sprawuje się o wiele lepiej w znajdywaniu puzzli gdy jest większa
			var q2 = q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate(8).Erode();

			Invoke(new Action(delegate { Progress(20, "Znajdowanie wszystkich konturów..."); }));
			var w3 = ExtensionMethods.FindContours(q2.Copy());

			Invoke(new Action(delegate { Progress(30, "Wybieranie puzzli..."); }));
			var avg = ExtensionMethods.CalculateAvreage(w3.Item1);
			var e4 = new VectorOfVectorOfPoint();
			for (var i = 0; i < w3.Item1.Size; i++)
			{
				if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);
			}

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

				var img = q1.Copy();

				img.ROI = r;

				puzzels.Add(img);

				q1.Rectangle(r, new MCvScalar(255, 0, 255));

				q1.PutText(x.ToString(), new Point(r.X + r.Width / 2, r.Y + r.Height / 2), new MCvScalar(255, 0, 255), FontFace.HersheySimplex, 8, 10);
			}

			Invoke(new Action(delegate { Progress(60); }));

			foreach (var puzzel in puzzels)
			{
				var q6 = puzzel.Convert<Gray, byte>().Copy().AdaptiveThreshold().Dilate();

				var w7 = ExtensionMethods.FindContours(q6);

				var max = new VectorOfVectorOfPoint();

				max.Push(w7.Item1[0]);

				for (var i = 0; i < w7.Item1.Size; i++)
				{
					if (!(CvInvoke.ContourArea(w7.Item1[i]) > CvInvoke.ContourArea(max[0]))) continue;
					max.Clear();
					max.Push(w7.Item1[i]);
				}

				Invoke(new Action(delegate { Progress(65); }));

				//TODO Optimalize this !!!
				//Usuwanie tła
				//var puzzelcopy = puzzel.Clone().FillPoly(max, new MCvScalar(0, 0, 255));
				//for (var j = 1; j < puzzelcopy.Cols; j++)
				//{
				//	for (var i = 1; i < puzzelcopy.Rows; i++)
				//	{
				//		if (!puzzelcopy[i, j].Equals(new Bgr(0, 0, 255)))
				//		{
				//			puzzel[i, j] = new Bgr(Color.Transparent);
				//		}
				//	}
				//}
			}

			Invoke(new Action(delegate { Progress(70); }));

			var k1 = q1.Copy();
			var surf = new SURF(600);

			//TODO zdjęcie z ułożonymi puzzlami
			var orginal = new Image<Bgr, byte>(ExtensionMethods.ImagePath); //tu powinna być ścieżka

			var copyOrginal = orginal.Copy();

			var dac1 = surf.DetectAndCompute(q1);
			var dac2 = surf.DetectAndCompute(copyOrginal);

			Features2DToolbox.DrawKeypoints(q1, dac1.Item1, k1, new Bgr(0, 255, 0));
			Features2DToolbox.DrawKeypoints(orginal, dac2.Item1, copyOrginal, new Bgr(0, 255, 0));

			Invoke(new Action(delegate { Progress(75); }));

			//dopasowywanie
			var matcher = new BFMatcher(DistanceType.L2);
			var matches = new VectorOfVectorOfDMatch();

			matcher.KnnMatch(dac1.Item2, matches, 2, null);

			matcher.Add(dac2.Item2);


			var result = new Mat();
			var res = new Image<Bgr, byte>(result.Bitmap);
			Features2DToolbox.DrawMatches(copyOrginal, dac2.Item1, k1, dac1.Item1, matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255));

			

			foreach (var puzzel in puzzels)
			{
				var dac3 = surf.DetectAndCompute(puzzel);

				var puzzelmatches = new VectorOfVectorOfDMatch();
				var Result = new Mat();

				matcher.KnnMatch(dac3.Item2, puzzelmatches, 3, null);
				
				Features2DToolbox.DrawMatches(copyOrginal, dac2.Item1, puzzel, dac2.Item1, puzzelmatches, Result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), null);
				var MachedPuzzel = new Image<Bgr, Byte>(Result.Bitmap);

				ExtensionMethods.ImageOut.Add(MachedPuzzel.ToBitmap());
				//  Bgr color = puzzel[0, 0]; //czytanie koloru (przydatne do układania kolorami)
				//System.Console.Out.Write(color.ToString());
			}

			Invoke(new Action(delegate { Progress(95, "Zapisywanie..."); }));

			ExtensionMethods.ImageOut.Add(res.ToBitmap());
			ExtensionMethods.ImageOut.Add(k1.ToBitmap());
			ExtensionMethods.ImageOut.Add(copyOrginal.ToBitmap());
			ExtensionMethods.ImageOut.Add(q1.ToBitmap());

			Invoke(new Action(delegate { Progress(100, "DONE"); }));

			watch.Stop();
			var elapsedMs = watch.Elapsed.TotalSeconds / 60;

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