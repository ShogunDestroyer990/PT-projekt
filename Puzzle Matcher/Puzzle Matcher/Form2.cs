using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Puzzle_Matcher
{
	public partial class ResultWindow : Form
	{
		public ResultWindow()
		{
			if(ExtensionMethods.ImageOut != null && ExtensionMethods.ImageOut.Count > 0)
			{
				Images = ExtensionMethods.ImageOut;

				Selected = 0;

				InitializeComponent();

				ImageOut.Image = ExtensionMethods.ResizeImage(Images[Selected], ImageOut.Width, ImageOut.Height);
			}
			else
			{
				MessageBox.Show("Something went wrong.", "Error");
				Close();
			}
		}

		private List<Bitmap> Images { get; }
		private int Selected { get; set; }

		private void ImageOut_Click(object sender, EventArgs e)
		{
			if(Selected < Images.Count - 1) Selected += 1;
			else Selected = 0;

			ImageOut.Image = ExtensionMethods.ResizeImage(Images[Selected], ImageOut.Width, ImageOut.Height);
		}
	}
}