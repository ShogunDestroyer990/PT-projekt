using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
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

			ImageIn.Image = ExtensionMethods.ResizeImage(new Bitmap(ExtensionMethods.ImagePath), ImageIn.Width, ImageIn.Height);

			if (ExtensionMethods.ImagePath != null || ExtensionMethods.ImagePath != "") ProcessImage.Enabled = true;
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
			if (ExtensionMethods.ImagePath == null) return;

			var preview = new Form
			{
				ClientSize = new Size(815, 615),
				Text = Resources.PreviewWindowName,
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

			#region Preview Logic

			var q1 = new Image<Bgr, byte>(ExtensionMethods.ImagePath);

			var w3 = ExtensionMethods.FindContours(q1.Copy().Convert<Gray, byte>().GaussBlur().AdaptiveThreshold().Dilate().Erode());

			var avg = ExtensionMethods.CalculateAvreage(w3.Item1, (double)prog.Value);
			var e4 = new VectorOfVectorOfPoint();
			for (var i = 0; i < w3.Item1.Size; i++) if (CvInvoke.ContourArea(w3.Item1[i]) > avg) e4.Push(w3.Item1[i]);

			var q5 = q1.Copy().PutText("Puzzles find: " + e4.Size, new Point(200, 250), new MCvScalar(255, 255, 255));

			var boundRect = new List<Rectangle>();

			for (var i = 0; i < e4.Size; i++) boundRect.Add(CvInvoke.BoundingRectangle(e4[i]));
			var x = 0;

			foreach (var r in boundRect)
			{
				x++;
				var img = q5.Copy();
				img.ROI = r;
				CvInvoke.Rectangle(q5, r, new MCvScalar(255, 0, 255), 10);
				CvInvoke.PutText
				(
					q5
					, x.ToString()
					, new Point(r.X + r.Width / 2, r.Y + r.Height / 2)
					, FontFace.HersheySimplex
					, 8
					, new MCvScalar(255, 0, 255)
					, 10);
			}

			#endregion
			
			image.Image = ExtensionMethods.ResizeImage(q5.ToBitmap(), 800, 600);
			

			preview.Controls.Add(image);
			preview.Icon = Resources.Icon;
			((System.ComponentModel.ISupportInitialize)(image)).EndInit();
			preview.ResumeLayout(false);
			preview.PerformLayout();

			preview.Show();

		}
	}
}