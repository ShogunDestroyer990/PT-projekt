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
using Puzzle_Matcher.Helpers;
using Puzzle_Matcher.Properties;

namespace Puzzle_Matcher.WinForms
{
	public partial class MainWindow : Form
	{
		/// <inheritdoc />
		/// <summary>
		///     Constructor of MainWindow. Initialize logic and appearance for MainWindow.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
		}

		private bool IsFirstTime { get; set; } = true;

		private Tuple<Bitmap, int> PreviewElement { get; set; }

		/// <summary>
		///     Opens file dialog to choose image then saves that image.
		///     <param name="sender">PictureBox object where user clicks.</param>
		///     <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Loads image path into memory.</returns>
		private void ImageIn_Click(object sender, EventArgs e)
		{
			ofd.FileName = string.Empty;

			if(ofd.ShowDialog() != DialogResult.OK) return;

			ExtensionMethods.ImagePath = ofd.FileName;

			new Thread
			(
				() =>
				{
					Invoke
					(
						new Action
						(
							() =>
							{
								ImageIn.Image = ExtensionMethods.ResizeImage
									(new Bitmap(ExtensionMethods.ImagePath), ImageIn.Width, ImageIn.Height);
							}));
				}).Start();

			if(ExtensionMethods.ImagePath != null || ExtensionMethods.ImagePath != "") ProcessImage.Enabled = true;

			PreviewElement = null;
		}

		private void PredictSizeOfPuzzles()
		{
			if(PreviewElement == null) return;
			X_axis.Value = Math.Floor((decimal) ( PreviewElement.Item2 / 2.0 ));
			Y_axis.Value = PreviewElement.Item2 / X_axis.Value;
		}

		/// <summary>
		///     Opens new window where loaded image is proccesing.
		///     <param name="sender">PictureBox object where user clicks.</param>
		///     <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Closing current window.</returns>
		private void ProcessImage_Click(object sender, EventArgs e)
		{
			if(ExtensionMethods.ImagePath == null)
			{
				MessageBox.Show("Przepraszamy, aby przejść dalej musisz wczytać obraz.", "Błąd");
				return;
			}

			if(PreviewElement == null)
			{
				MessageBox.Show
				(
					"Przepraszamy, aby przejść dalej musisz wygenerować podgląd obrazka."
					+ Environment.NewLine
					+ Environment.NewLine
					+ "W tem celu naciśni przycik \"Preview\"."
					, "Błąd");
				return;
			}

			if(PreviewElement.Item2 % 2 == 1)
			{
				MessageBox.Show
				(
					"Przepraszamy, nie można ułożyć puzzli z nieparzystej ilości elementów."
					+ Environment.NewLine
					+ Environment.NewLine
					+ "Spróbuj zmienić ustawienia wielkości konturu."
					, "Błąd");
				return;
			}

			if(ExtensionMethods.OrginalImagePath == null)
			{
				MessageBox.Show("Przepraszamy, aby przejść dalej musisz wczytać orginalny obraz.", "Błąd");
				return;
			}

			var t = new Thread(StartNewStaThread)
			{
#pragma warning disable 618
				ApartmentState = ApartmentState.STA
#pragma warning restore 618
			};

			t.Start();

			Close();
		}

		private void StartNewStaThread()
		{
			Application.Run
			(
				new WorkInProgress
					((int) X_axis.Value, (int) Y_axis.Value, (double) prog.Value / 100, (double) MatchDistance.Value));
		}

		/// <summary>
		///     Opens file dialog to choose image then saves that image.
		///     <param name="sender">PictureBox object where user clicks.</param>
		///     <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Loads orginal image path into memory.</returns>
		private void OrginalImg_Click(object sender, EventArgs e)
		{
			ofd.FileName = string.Empty;

			if(ofd.ShowDialog() != DialogResult.OK) return;

			ExtensionMethods.OrginalImagePath = ofd.FileName;

			OrginalImg.Image = ExtensionMethods.ResizeImage
				(new Bitmap(ExtensionMethods.OrginalImagePath), OrginalImg.Width, OrginalImg.Height);

			if(ExtensionMethods.OrginalImagePath != null || ExtensionMethods.OrginalImagePath != "") ProcessImage.Enabled = true;
		}

		private void Preview_Click(object sender, EventArgs e)
		{
			if(ExtensionMethods.ImagePath == null) return;

			var preview = new Form
			{
				ClientSize = new Size(815, 615)
			};

			var image = new PictureBox();
			( (ISupportInitialize) image ).BeginInit();
			image.Enabled = false;
			image.Location = new Point(13, 13);
			image.Name = "ProcessImage";
			image.Size = new Size(800, 600);
			image.TabIndex = 1;
			image.TabStop = false;
			image.InitialImage = null;

			PreviewElement = CreatePreviewImage(ExtensionMethods.ImagePath, (double) prog.Value / 100);
			if(IsFirstTime)
			{
				PredictSizeOfPuzzles();
				IsFirstTime = false;
			}

			preview.Text = @"Preview - Puzzles find: " + PreviewElement.Item2;

			image.Image = ExtensionMethods.ResizeImage(PreviewElement.Item1, 800, 600);

			preview.Controls.Add(image);
			preview.Icon = Resources.Icon;
			( (ISupportInitialize) image ).EndInit();
			preview.ResumeLayout(false);
			preview.PerformLayout();

			preview.Show();
		}

		private Tuple<Bitmap, int> CreatePreviewImage(string imgPath, double val)
		{
			var q1 = new Image<Bgr, byte>(imgPath);

			var w3 = ExtensionMethods.FindContours
				(q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate(8).Erode());

			var avg = ExtensionMethods.CalculateAvreage(w3.Item1, val);

			var e4 = new VectorOfVectorOfPoint();
			for(var i = 0; i < w3.Item1.Size; i++) if(CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);

			var boundRect = new List<Rectangle>();

			for(var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));

			var puzzelCounter = 0;

			var avgX = new int[boundRect.Count];
			var avgY = new int[boundRect.Count];

			foreach(var r in boundRect)
			{
				avgX[puzzelCounter] = r.X;
				avgY[puzzelCounter] = r.Y;
				puzzelCounter++;
				q1 = q1.Rectangle
					(r, new MCvScalar(250, 0, 250))
					.PutText
					(
						puzzelCounter.ToString()
						, new Point(r.X + r.Width / 2, r.Y + r.Height / 2)
						, new MCvScalar(255, 0, 255)
						, FontFace.HersheySimplex
						, 10
						, 20);
			}

			var assumedConfiguration = ExtensionMethods.AssumePuzzleConfiguration(avgX, avgY);

			X_axis.Value = assumedConfiguration[1];
			Y_axis.Value = assumedConfiguration[0];

			return new Tuple<Bitmap, int>(q1.ToBitmap(), puzzelCounter);
		}
	}
}