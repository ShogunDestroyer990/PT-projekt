﻿namespace Puzzle_Matcher
{
	partial class WorkInProgress
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.Worker = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(12, 12);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(800, 30);
			this.progressBar.TabIndex = 0;
			// 
			// Worker
			// 
			this.Worker.WorkerReportsProgress = true;
			this.Worker.WorkerSupportsCancellation = true;
			this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Worker_DoWork);
			this.Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Worker_ProgressChanged);
			this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
			// 
			// WorkInProgress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(824, 54);
			this.Controls.Add(this.progressBar);
			this.Name = "WorkInProgress";
			this.Text = "Your image is processing.";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar;
		private System.ComponentModel.BackgroundWorker Worker;
	}
}