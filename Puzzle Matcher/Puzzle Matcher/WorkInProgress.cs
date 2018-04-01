using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Puzzle_Matcher
{
	public partial class WorkInProgress : Form
	{
		public WorkInProgress()
		{
			InitializeComponent();

			Worker.RunWorkerAsync();
		}

		private void Progress(int progresPercent, string description = "")
		{
			Worker.ReportProgress(progresPercent);
			Description.Text = description;
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			Invoke(new Action(delegate { Progress(0); }));

			//TODO: Calls stack with pattern
			//ExtensionMethods.Method();
			//Invoke(new Action(delegate { Progress(int, string); }));

			Invoke(new Action(delegate { Progress(50, "Finding edges of puzzles..."); }));

			ExtensionMethods.FindEdges();

			Thread.Sleep(1000);

			Invoke(new Action(delegate { Progress(100, "DONE"); }));
		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			new Thread(() => { Application.Run(new ResultWindow()); }).Start();

			Close();
		}
	}
}