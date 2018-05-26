using System.ComponentModel;
using System.Windows.Forms;

namespace Puzzle_Matcher.WinForms
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
			this.SaveToFolder = new System.Windows.Forms.Button();
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
			// SaveToFolder
			// 
			this.SaveToFolder.Location = new System.Drawing.Point(13, 557);
			this.SaveToFolder.Name = "SaveToFolder";
			this.SaveToFolder.Size = new System.Drawing.Size(759, 62);
			this.SaveToFolder.TabIndex = 1;
			this.SaveToFolder.Text = "Zapisz wszystko do folderu ...";
			this.SaveToFolder.UseVisualStyleBackColor = true;
			this.SaveToFolder.Click += new System.EventHandler(this.SaveToFolder_Click);
			// 
			// ResultWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 622);
			this.Controls.Add(this.SaveToFolder);
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
		private Button SaveToFolder;
	}
}