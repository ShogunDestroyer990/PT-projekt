using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Puzzle_Matcher.Helpers;

namespace Puzzle_Matcher.WinForms
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
				MessageBox.Show(@"Something went wrong.", @"Error");
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

		private void SaveToFolder_Click(object sender, EventArgs e)
		{
			Invoke(new Action(
				() =>
				{
					using (var fbd = new FolderBrowserDialog()
					{
						Description = @"Wybierz folder do zapisania obrazków"
						,
						SelectedPath = string.Empty
						,
						RootFolder = Environment.SpecialFolder.Desktop
						,
						ShowNewFolderButton = true
					})
					{
						if (fbd.ShowDialog() != DialogResult.OK) return;

						if (string.IsNullOrEmpty(fbd.SelectedPath) || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

						var ver = new bool[Images.Count];
						for(var index = 0; index < ver.Length; index++)
						{
							ver[index] = false;
						}

						for (var index = 0; index < Images.Count; index++)
						{
							var image = Images[index];
							var path = fbd.SelectedPath + "\\" + index + ".png";
							image.Save(path, ImageFormat.Png);
							if(File.Exists(path)) ver[index] = true;
						}

						if(ver.Any(b => false))
						{
							MessageBox.Show
							(
								@"Błąd zapisu plików"
								, @"Niestety z nieznanych nam przyczyn nie udało się zapisać wszystkich obrazków poprawnie."
								  + Environment.NewLine
								  + @"Spróbuj zapisać jeszcze raz lub zrestartować program."
								  + Environment.NewLine
								  + Environment.NewLine
								  + @"Jeżeli problem dalej występuje, napisz do nas na email:"
								  + @"mail@example.com"
							);
						}
						else
						{
							MessageBox.Show
							(
								@"Gratulacje",
								@"Udało się zapisać wszystkie pliki."
							);
						}

					}

				}));
			


			
		}
	}
}