using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

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
			ImageIn.Image = new Bitmap(ImageIn.Width, ImageIn.Height).DrawSymbol("Wybierz obraz na którym chcesz ułożyć puzzle", new SolidBrush(Color.Gray), new Font(FontFamily.GenericSerif, 15), new SolidBrush(Color.Black));
			OrginalImg.Image = new Bitmap(ImageIn.Width, ImageIn.Height).DrawSymbol("Wybierz obraz orginalny (ułożony)", new SolidBrush(Color.Gray), new Font(FontFamily.GenericSerif, 15), new SolidBrush(Color.Black));
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

			if(ExtensionMethods.ImagePath != null || ExtensionMethods.ImagePath != "") ProcessImage.Enabled = true;
		}

		/// <summary>
		///     Opens new window where loaded image is proccesing.
		///     <param name="sender">PictureBox object where user clicks.</param>
		///     <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Closing current window.</returns>
		private void ProcessImage_Click(object sender, EventArgs e)
		{
			if(ExtensionMethods.ImagePath == null) return;
			new Thread(() => { Application.Run(new WorkInProgress((int)X_axis.Value, (int)Y_axis.Value)); }).Start();

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
	}
}