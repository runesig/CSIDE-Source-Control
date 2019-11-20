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
        public static int Excecute(string directory, string command, out string output)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = directory,
                    FileName = "git",
                    Arguments = command,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                Process gitProcess = Process.Start(startInfo);

                string standardOutput = gitProcess.StandardOutput.ReadToEnd();
                string standardError = gitProcess.StandardError.ReadToEnd();
                gitProcess.WaitForExit();

                output = CheckOutputString(standardOutput, standardError);

                return gitProcess.ExitCode;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public static void Wait()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        private static string CheckOutputString(string standardOutput, string standardError)
        {
            if (standardError.Contains("fatal:"))
                throw new Exception(standardError);
            
            if((string.IsNullOrEmpty(standardOutput)) && (!string.IsNullOrEmpty(standardError)))
                    return standardError;

            if ((!string.IsNullOrEmpty(standardOutput)) && (string.IsNullOrEmpty(standardError)))
                return standardOutput;

            return string.Format("{0}\n{1}", standardOutput, standardError);
        }
    }
}
