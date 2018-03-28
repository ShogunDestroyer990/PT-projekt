using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

namespace Puzzle_Matcher
{
	public partial class ResultWindow : Form
	{
		public ResultWindow()
		{
			InitializeComponent();
              ImageOut.Image = ExtensionMethods.ResizeImage(((Bitmap)ExtensionMethods.ImageOut),ImageOut.Width,ImageOut.Height);
              ImageOut2.Image = ExtensionMethods.ResizeImage(((Bitmap)ExtensionMethods.ImageOut2), ImageOut2.Width, ImageOut2.Height);
         //  ImageOut.Image = (Bitmap)ExtensionMethods.ImageOut;
           //ImageOut2.Image = (Bitmap)ExtensionMethods.ImageOut2;

        }

		private void ImageOut_Click(object sender, EventArgs e)
		{

		}

        private void ImageOut2_Click(object sender, EventArgs e)
        {

        }
    }
}
