namespace Puzzle_Matcher
{
	partial class MainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.ImageIn = new System.Windows.Forms.PictureBox();
			this.ProcessImage = new System.Windows.Forms.PictureBox();
			this.ofd = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.ImageIn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ProcessImage)).BeginInit();
			this.SuspendLayout();
			// 
			// ImageIn
			// 
			this.ImageIn.Image = global::Puzzle_Matcher.Properties.Resources.ButtonLoadImage;
			this.ImageIn.InitialImage = null;
			this.ImageIn.Location = new System.Drawing.Point(13, 13);
			this.ImageIn.Name = "ImageIn";
			this.ImageIn.Size = new System.Drawing.Size(759, 431);
			this.ImageIn.TabIndex = 0;
			this.ImageIn.TabStop = false;
			this.ImageIn.Click += new System.EventHandler(this.ImageIn_Click);
			// 
			// ProcessImage
			// 
			this.ProcessImage.Image = global::Puzzle_Matcher.Properties.Resources.ButtonProcessImage;
			this.ProcessImage.InitialImage = null;
			this.ProcessImage.Location = new System.Drawing.Point(12, 450);
			this.ProcessImage.Name = "ProcessImage";
			this.ProcessImage.Size = new System.Drawing.Size(760, 100);
			this.ProcessImage.TabIndex = 1;
			this.ProcessImage.TabStop = false;
			this.ProcessImage.Click += new System.EventHandler(this.ProcessImage_Click);
			// 
			// ofd
			// 
			this.ofd.DefaultExt = "%HOMEPATH%";
			this.ofd.FileName = "WybierzZdjecie";
			this.ofd.Filter = "PNG Image (.png)|*.png|GIF Image (.gif)|*.gif|JPG Image (.jpg) |*.jpg|BITMAP Imag" +
    "e (.bmp)|*.bmp|All files (.*)|*.*";
			this.ofd.Title = "Wybierz zdjęcie";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.ProcessImage);
			this.Controls.Add(this.ImageIn);
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainWindow";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Okno główne";
			((System.ComponentModel.ISupportInitialize)(this.ImageIn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ProcessImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox ImageIn;
		private System.Windows.Forms.PictureBox ProcessImage;
		private System.Windows.Forms.OpenFileDialog ofd;
	}
}

