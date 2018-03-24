using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Puzzle_Matcher.Properties;

namespace Puzzle_Matcher
{
	public partial class MainWindow : Form
	{
		/// <inheritdoc />
		/// <summary>
		/// Constructor of MainWindow. Initialize logic and appearance for MainWindow.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Opens file dialog to choose image then saves that image.
		/// <param name="sender">PictureBox object where user clicks.</param>
		/// <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Closing current window.</returns>
		private void ImageIn_Click(object sender, EventArgs e)
		{
			if(ofd.ShowDialog() != DialogResult.OK) return;

			ExtensionMethods.ImagePath = ofd.FileName;

			ImageIn.Image = ExtensionMethods.ResizeImage(new Bitmap(ofd.FileName), ImageIn.Width, ImageIn.Height);

		}

		/// <summary>
		/// Opens new window where loaded image is proccesing.
		/// <param name="sender">PictureBox object where user clicks.</param>
		/// <param name="e">Provides information about click itself.</param>
		/// </summary>
		/// <returns>Closing current window.</returns>
		private void ProcessImage_Click(object sender, EventArgs e)
		{
			new Thread(() =>
			{
				Application.Run(new WorkInProgress());
			}).Start();

			Close();
		}
	}
}
