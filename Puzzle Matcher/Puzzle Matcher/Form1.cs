using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Puzzle_Matcher.Properties;

namespace Puzzle_Matcher
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

		/// <summary>
		///     Opens file dialog to choose image then saves that image.
		///     <param name="sender">PictureBox object where user clicks.</param>
		///     <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Loads image path into memory.</returns>
		private void ImageIn_Click(object sender, EventArgs e)
		{
			ofd.FileName = string.Empty;

			if (ofd.ShowDialog() != DialogResult.OK) return;

			ExtensionMethods.ImagePath = ofd.FileName;

			new Thread(() =>{Invoke(new Action(() =>{ImageIn.Image = ExtensionMethods.ResizeImage(new Bitmap(ExtensionMethods.ImagePath), ImageIn.Width, ImageIn.Height);}));}).Start();

			//new Thread(() =>{Invoke(new Action(PredictSizeOfPuzzles));}).Start();

			if (ExtensionMethods.ImagePath != null || ExtensionMethods.ImagePath != "") ProcessImage.Enabled = true;
		}

		private void PredictSizeOfPuzzles()
		{
			PreviewElement = CreatePreviewImage(ExtensionMethods.ImagePath, (double)prog.Value / 100);
			X_axis.Value = Math.Floor((decimal)(PreviewElement.Item2 / 2));
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
			if (ExtensionMethods.ImagePath == null || ExtensionMethods.OrginalImagePath == null) return;
			new Thread(() => { Application.Run(new WorkInProgress((int)X_axis.Value, (int)Y_axis.Value, ((double)prog.Value / 100))); }).Start();

			Close();
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

			if (ofd.ShowDialog() != DialogResult.OK) return;

			ExtensionMethods.OrginalImagePath = ofd.FileName;

			OrginalImg.Image = ExtensionMethods.ResizeImage(new Bitmap(ExtensionMethods.OrginalImagePath), OrginalImg.Width, OrginalImg.Height);

			if (ExtensionMethods.OrginalImagePath != null || ExtensionMethods.OrginalImagePath != "") ProcessImage.Enabled = true;
		}

		private void Preview_Click(object sender, EventArgs e)
		{
			if (ExtensionMethods.ImagePath == null || PreviewElement == null) return;

			var preview = new Form
			{
				ClientSize = new Size(815, 615),
			};

			var image = new PictureBox();
			((System.ComponentModel.ISupportInitialize)(image)).BeginInit();
			image.Enabled = false;
			image.Location = new Point(13, 13);
			image.Name = "ProcessImage";
			image.Size = new Size(800, 600);
			image.TabIndex = 1;
			image.TabStop = false;
			image.InitialImage = null;

			preview.Text = @"Preview - Puzzles find: " + PreviewElement.Item2;
			image.Image = ExtensionMethods.ResizeImage(PreviewElement.Item1, 800, 600);
			

			preview.Controls.Add(image);
			preview.Icon = Resources.Icon;
			((System.ComponentModel.ISupportInitialize)(image)).EndInit();
			preview.ResumeLayout(false);
			preview.PerformLayout();

			preview.Show();

		}

		private Tuple<Bitmap, int> PreviewElement { get; set; } = null;

		private Tuple<Bitmap, int> CreatePreviewImage(string imgPath, double val)
		{
			var q1 = new Image<Bgr, byte>(imgPath);

			var w3 = ExtensionMethods.FindContours(q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate(8).Erode());

			var avg = ExtensionMethods.CalculateAvreage(w3.Item1, val);

			var e4 = new VectorOfVectorOfPoint();
			for (var i = 0; i < w3.Item1.Size; i++) if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);

			var boundRect = new List<Rectangle>();

			for (var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));

			var puzzelCounter = 0;

			foreach (var r in boundRect)
			{
				puzzelCounter++;
				q1 = q1.Rectangle(r, new MCvScalar(250, 0, 250)).PutText(puzzelCounter.ToString(), new Point(r.X + r.Width / 2, r.Y + r.Height / 2), new MCvScalar(255, 0, 255), FontFace.HersheySimplex, 10, 20);
			}

			return new Tuple<Bitmap, int>(q1.ToBitmap(), puzzelCounter);
		}
	}
}