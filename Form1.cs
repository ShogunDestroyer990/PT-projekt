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
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

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
            /*
                OpenFileDialog Openfile = new OpenFileDialog();
            if (Openfile.ShowDialog() == DialogResult.OK)
            {
                Image<Bgr, Byte> My_Image = new Image<Bgr, Byte>(Openfile.FileName);
                Image<Bgr, Byte> inewcolor = My_Image;
                Image<Gray, Byte> ibw = My_Image.Convert<Gray, Byte>();

                Image<Gray, Byte> ibw1 = ibw.Canny(100, 200);



                ibw1._Dilate(1);
                ibw1._Erode(1);



               // ImageIn.Image = My_Image.ToBitmap();
                ImageIn.Image = ibw1.ToBitmap();
            }
             */



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
