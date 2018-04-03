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
			var q2 = q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate().Erode();


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
				if(x == boundRect.Count/3) Invoke(new Action(delegate { Progress(60); }));
				if (x == (2*boundRect.Count) / 3) Invoke(new Action(delegate { Progress(70); }));
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
			ExtensionMethods.ImageOut.Add(q1.ToBitmap());
			ExtensionMethods.ImageOut.Add(q5.ToBitmap());


			Invoke(new Action(delegate { Progress(100, "DONE"); }));

			Thread.Sleep(500);
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