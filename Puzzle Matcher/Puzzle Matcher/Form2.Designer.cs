using System.ComponentModel;
using System.Windows.Forms;

namespace Puzzle_Matcher
{
	partial class ResultWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultWindow));
			this.ImageOut = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.ImageOut)).BeginInit();
			this.SuspendLayout();
			// 
			// ImageOut
			// 
			this.ImageOut.Location = new System.Drawing.Point(12, 12);
			this.ImageOut.Name = "ImageOut";
			this.ImageOut.Size = new System.Drawing.Size(760, 538);
			this.ImageOut.TabIndex = 0;
			this.ImageOut.TabStop = false;
			this.ImageOut.Click += new System.EventHandler(this.ImageOut_Click);
			// 
			// ResultWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.ImageOut);
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ResultWindow";
			this.Text = "Result Window";
			((System.ComponentModel.ISupportInitialize)(this.ImageOut)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private PictureBox ImageOut;
	}
}