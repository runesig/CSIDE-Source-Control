using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSIDESourceControl.Client.Git
{
    public class GitProcess
    {
        public static string Excecute(string directory, string command)
        {
            string output = string.Empty;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WorkingDirectory = directory,
                FileName = "git",
                Arguments = command,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process gitProcess = Process.Start(startInfo);
            output = gitProcess.StandardOutput.ReadToEnd();
            Thread.Sleep(TimeSpan.FromSeconds(1));

            return output;
        }
    }
}
