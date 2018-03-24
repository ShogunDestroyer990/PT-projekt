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

namespace Puzzle_Matcher
{
	public partial class WorkInProgress : Form
	{
		public WorkInProgress()
		{
			InitializeComponent();

			Worker.RunWorkerAsync();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			if(!( sender is BackgroundWorker worker )) return;

			worker.ReportProgress(0);

			//TODO: Calls stack with pattern
			//ExtensionMethods.Method();
			//worker.ReportProgress(Progress);


			//DEBUG ONLY
			Thread.Sleep(5000);


			worker.ReportProgress(100);
		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			new Thread(() =>
			{
				Application.Run(new ResultWindow());
			}).Start();

			Close();
		}
	}
}
