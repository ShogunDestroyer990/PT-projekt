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
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Puzzle_Matcher
{
	public partial class WorkInProgress : Form
	{
		public WorkInProgress(int x_ax, int y_ax, double prog)
		{
			InitializeComponent();
			X_ax = x_ax;
			Y_ax = y_ax;
			Prog = prog;

			Worker.RunWorkerAsync();
		}

		private int X_ax { get; }
		private int Y_ax { get; }

		private double Prog { get; }

		private void Progress(int progresPercent, string description = null)
		{
			if (description != null) Description.Text = description;
			Worker.ReportProgress(progresPercent);
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			//Invoke(new Action(delegate { Progress(int, string); }));

			#region Indentyfikacja puzzli

			Invoke(new Action(delegate { Progress(0, "Wczytywanie obrazka."); }));
			var q1 = new Image<Bgr, byte>(ExtensionMethods.ImagePath);

			Invoke(new Action(delegate { Progress(33, "Wstępna obróbka obrazka."); }));
			var w3 = ExtensionMethods.FindContours(q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate(8).Erode());

			var avg = ExtensionMethods.CalculateAvreage(w3.Item1, Prog);
			var e4 = new VectorOfVectorOfPoint();
			for (var i = 0; i < w3.Item1.Size; i++) if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);
			var boundRect = new List<Rectangle>();
			for (var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));
			

			var puzzels = new List<Image<Bgr, byte>>();

			Invoke(new Action(delegate { Progress(50, "Znajdowanie puzzli"); }));

			var puzzleCount = 0;
			foreach (var r in boundRect)
			{
				puzzleCount++;
				var img = q1.Copy();

				img.ROI = r;
				puzzels.Add(img.Copy());

				q1 = q1.Rectangle(r, new MCvScalar(255, 0, 255));
				q1 = q1.PutText(puzzleCount.ToString(), new Point(r.X + r.Width / 2, r.Y + r.Height / 2), new MCvScalar(255, 0, 255), FontFace.HersheySimplex, 10, 20);

			}

			#endregion

			#region Cechy wspólne

			Invoke(new Action(delegate { Progress(66, "Detekcja cech wspólnych."); }));
			var surf = new SURF(920);
			var puzzelCounter = puzzels.Count();
			var avgPuzellXPoints = new double[puzzelCounter];
			var avgPuzellYPoints = new double[puzzelCounter];
			puzzelCounter = 0;

			Invoke(new Action(delegate { Progress(70, "Znajdowanie puktów charakterystycznych dla orginalnego obrazu."); }));
			var orginal = new Image<Bgr, byte>(ExtensionMethods.OrginalImagePath);
			var copyOrginal = orginal.Copy();
			var orginalFeatures = ExtensionMethods.DetectAndCompute(surf, copyOrginal, false);
			var odesc = orginalFeatures.Item1;
			var orginalKeypoints = orginalFeatures.Item2;

			#endregion

			#region Dopasowanie

			Invoke(new Action(delegate { Progress(80, "Próba dopasowania puzzli."); }));

			var matcher = new BFMatcher(DistanceType.L2);
			matcher.Add(odesc);

			foreach (var puzzel in puzzels)
			{
				var pdesc = surf.DetectAndCompute(puzzel);
				var puzzelmatches = matcher.KnnMatch(pdesc, 3);

				double x = 0;
				double y = 0;
				var count = 0;

				for (var i = 0; i < puzzelmatches.Size; i++)
				{
					var arrayOfMatches = puzzelmatches[i].ToArray();

					foreach (var match in arrayOfMatches)
					{
						if (!(match.Distance > 0.1)) continue; //IMPORTANT
						x += orginalKeypoints[match.TrainIdx].Point.X;
						y += orginalKeypoints[match.TrainIdx].Point.Y;
						count++;
					}
				}

				x = x / count;
				y = y / count;

				avgPuzellXPoints[puzzelCounter] = x;
				avgPuzellYPoints[puzzelCounter] = y;
				puzzelCounter++;
			}


			#endregion

			#region Układanie puzzli

			Invoke(new Action(delegate { Progress(90, "Układanie puzzli w właściwej kolejnoścu."); }));

			var resultTab = ExtensionMethods.PlacePuzzels(X_ax, Y_ax, avgPuzellXPoints, avgPuzellYPoints);

			Invoke(new Action(delegate { Progress(90, "Tworzenie obrazka końcowego."); }));
			var fp = ExtensionMethods.GenerateFinalpicture(X_ax, Y_ax, resultTab, puzzels);

			var finalword = "Puzzle należy ułożyć w kolejności:" + Environment.NewLine;
			for (var i = 0; i < puzzelCounter; i++)
			{
				resultTab[i]++; //przetwarzając przetwarzałem od zera a puzzle są od 1 ..więc
				finalword += resultTab[i];
				finalword += " ";
				if (i != 0 && ((i+1) % (X_ax)) == 0) finalword += Environment.NewLine;
			}

			var solution = new Bitmap(q1.Width / 2, q1.Height / 2);
			solution.DrawSymbol(finalword, new SolidBrush(Color.Gray), new Font(FontFamily.GenericSerif, 40), new SolidBrush(Color.Black));

			#endregion


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