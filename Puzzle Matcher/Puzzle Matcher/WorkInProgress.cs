using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

namespace Puzzle_Matcher
{
	public partial class WorkInProgress : Form
	{
		public WorkInProgress(int x_ax, int y_ax)
		{
			InitializeComponent();
			X_ax = x_ax;
			Y_ax = y_ax;

			Worker.RunWorkerAsync();
		}

		private int X_ax { get; }
		private int Y_ax { get; }

		private void Progress(int progresPercent, string description = null)
		{
			if (description != null) Description.Text = description;
			Worker.ReportProgress(progresPercent);
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			//Invoke(new Action(delegate { Progress(int, string); }));

			Invoke(new Action(delegate { Progress(0, "Wczytywanie obrazka."); }));
			var q1 = new Image<Bgr, byte>(ExtensionMethods.ImagePath);

			Invoke(new Action(delegate { Progress(33, "Wstępna obróbka obrazka."); }));
			var w3 = ExtensionMethods.FindContours(q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate(8).Erode());

			var avg = ExtensionMethods.CalculateAvreage(w3.Item1);
			var e4 = new VectorOfVectorOfPoint();
			for (var i = 0; i < w3.Item1.Size; i++) if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);

			//var q5 = q1.Copy().MarkCountours(e4, new MCvScalar(255, 0, 0)).PutText("Puzzles find: " + e4.Size, new Point(200, 250), new MCvScalar(255, 255, 255));

			var boundRect = new List<Rectangle>();

			for (var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));
			var x = 0;

			var puzzels = new List<Image<Bgr, byte>>();

			Invoke(new Action(delegate { Progress(50, "Znajdowanie puzzli"); }));
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

			var puzzelCounter = 0;

			foreach (var puzzel in puzzels)
			{
				puzzelCounter++;

				Image<Gray, Byte> gray = puzzel.Convert<Gray, Byte>();
				var hierarchy = new Mat();
				var contours = new VectorOfVectorOfPoint();
				Image<Gray, Byte> Imgout = gray;

				CvInvoke.AdaptiveThreshold(gray, Imgout, 250, AdaptiveThresholdType.MeanC, ThresholdType.BinaryInv, 39, 4);
				Imgout._Dilate(5);

				CvInvoke.FindContours(Imgout, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

				var Max = new VectorOfVectorOfPoint();
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
			}

			var avgPuzellXPoints = new double[puzzelCounter];
			var avgPuzellYPoints = new double[puzzelCounter];
			puzzelCounter = 0;

			Invoke(new Action(delegate { Progress(66, "Detekcja cech wspólnych."); }));

			var k1 = q1.Copy(); //dla wybranego obrazka
			var surf = new SURF(920);
			var keypoints = new VectorOfKeyPoint(); //keyponty
			var kdesc = new UMat();


			Invoke(new Action(delegate { Progress(70, "Znajdowanie puktów charakterystycznych dla badanego obrazu."); }));


			surf.DetectAndCompute(q1, null, keypoints, kdesc, false);

			Features2DToolbox.DrawKeypoints(q1, keypoints, k1, new Bgr(0, 255, 0));
			var orginal = new Image<Bgr, byte>(ExtensionMethods.OrginalImagePath); 
			var copyOrginal = orginal.Copy();
			var orginalKeypoints = new VectorOfKeyPoint();
			var odesc = new UMat();

			Invoke(new Action(delegate { Progress(75, "Znajdowanie puktów charakterystycznych dla orginalnego obrazu."); }));
			surf.DetectAndCompute(copyOrginal, null, orginalKeypoints, odesc, false);
			

			Features2DToolbox.DrawKeypoints(orginal, orginalKeypoints, copyOrginal, new Bgr(0, 255, 0));



			Invoke(new Action(delegate { Progress(80, "Próba dopasowania puzzli."); }));


			var matcher = new BFMatcher(DistanceType.L2);
			var matches = new VectorOfVectorOfDMatch();
			matcher.Add(odesc);
			matcher.KnnMatch(kdesc, matches, 2, null);

			var result = new Mat();

			Features2DToolbox.DrawMatches(copyOrginal, orginalKeypoints, k1, keypoints, matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), null);



			foreach (var puzzel in puzzels)
			{
				var puzzelpoints = new VectorOfKeyPoint(); //keyponty
				var pdesc = new UMat();
				surf.DetectAndCompute(puzzel, null, puzzelpoints, pdesc, false);

				var puzzelmatches = new VectorOfVectorOfDMatch();
				matcher.KnnMatch(pdesc, puzzelmatches, 3, null);

				var filteredpuzzelmatches = new VectorOfVectorOfDMatch();
				var filteredmatch = new VectorOfDMatch();

				double X = 0;
				double Y = 0;
				var counter = 0;
				var count = 0;

				for (int i = 0; i < puzzelmatches.Size; i++)
				{
					var arrayOfMatches = puzzelmatches[i].ToArray();
					var filterArray = new MDMatch[arrayOfMatches.Length];

					foreach (var match in arrayOfMatches)
					{
						if(!( match.Distance > 0.5 )) continue;
						X += orginalKeypoints[match.TrainIdx].Point.X;
						Y += orginalKeypoints[match.TrainIdx].Point.Y;
						filterArray[counter] = match;
						counter++;
						count++;
					}
					filteredmatch.Push(filterArray);
					counter = 0;
				}
				filteredpuzzelmatches.Push(filteredmatch);

				X = X / count;
				Y = Y / count;

				avgPuzellXPoints[puzzelCounter] = X;
				avgPuzellYPoints[puzzelCounter] = Y;
				puzzelCounter++;

				var Result = new Mat();

				Features2DToolbox.DrawMatches(copyOrginal, orginalKeypoints, puzzel, puzzelpoints, filteredpuzzelmatches, Result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), null);
			}


			Invoke(new Action(delegate { Progress(90, "Układanie puzzli w właściwej kolejnoścu."); }));

			//#Puzzles in x-ayx, y-ax
			var resultTab = ExtensionMethods.PlacePuzzels(X_ax, Y_ax, avgPuzellXPoints, avgPuzellYPoints);

			Invoke(new Action(delegate { Progress(90, "Tworzenie obrazka końcowego."); }));
			var fp = ExtensionMethods.generateFinalpicture(X_ax, Y_ax, resultTab, puzzels);

			var finalword = "Puzzle należy ułożyć w kolejności:" + Environment.NewLine;
			for (var i = 0; i < puzzelCounter; i++)
			{
				resultTab[i]++; //przetwarzając przetwarzałem od zera a puzzle są od 1 ..więc
				finalword += resultTab[i];
				finalword += " ";
				if(i == X_ax-1) finalword += Environment.NewLine;
			}

			var solution = new Bitmap(q1.Width/2, q1.Height/2);
			solution.DrawSymbol(finalword, new SolidBrush(Color.Gray), new Font(FontFamily.GenericSerif, 40), new SolidBrush(Color.Black));

			Invoke(new Action(delegate { Progress(99, "Zapisywanie postępów."); }));

			ExtensionMethods.ImageOut.Add(q1.ToBitmap());
			ExtensionMethods.ImageOut.Add(solution);
			ExtensionMethods.ImageOut.Add(fp.ToBitmap());

			
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