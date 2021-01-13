using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace EasyMC
{
	internal class ProcessRunner
	{
		private string command;

		private string args;

		public int ExitCode;

		public string Output;

		public string Error;

		public ProcessRunner(string command, string args)
		{
			this.command = command;
			this.args = args;
		}

		public void run(int timeToWait = 0)
		{
			Process process = new Process();
			process.StartInfo.FileName = this.command;
			process.StartInfo.Arguments = this.args;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.WaitForExit();
			this.ExitCode = process.ExitCode;
			this.Output = process.StandardOutput.ReadToEnd();
			this.Error = process.StandardError.ReadToEnd();
			Thread.Sleep(timeToWait);
		}
	}
}