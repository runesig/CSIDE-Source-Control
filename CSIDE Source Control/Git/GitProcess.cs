using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CSIDESourceControl.Git
{
    public class GitResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
    }
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

        public static List<string> CheckModifiedFilesFromOutput(string gitOutput)
        {
            List<string> modifiedFiles = new List<string>();

            using (StringReader reader = new StringReader(gitOutput))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    CheckFileExtension(modifiedFiles, line, ".txt");
                    CheckFileExtension(modifiedFiles, line, "Table/");
                    CheckFileExtension(modifiedFiles, line, "Page/");
                    CheckFileExtension(modifiedFiles, line, "Form/");
                    CheckFileExtension(modifiedFiles, line, "Codeunit/");
                    CheckFileExtension(modifiedFiles, line, "Report/");
                    CheckFileExtension(modifiedFiles, line, "Dataport/");
                    CheckFileExtension(modifiedFiles, line, "XMLport/");
                    CheckFileExtension(modifiedFiles, line, "Query/");
                    CheckFileExtension(modifiedFiles, line, "MenuSuite/");
                }
            }

            return modifiedFiles;
        }

        private static void CheckFileExtension(List<string> modifiedFiles, string line, string extension)
        {
            if (line.Contains(extension))
            {
                DeleteVerbsFromLine(ref line);

                string filename = line.Trim();

                bool containsItem = modifiedFiles.Any(item => item == filename);

                if(!containsItem)
                    modifiedFiles.Add(filename);
            }
        }

        private static void DeleteVerbsFromLine(ref string line)
        {
            line = line.Replace("modified:", string.Empty).Trim();
            line = line.Replace("deleted:", string.Empty).Trim();
            line = line.Replace("added:", string.Empty).Trim();
        }

        public static async Task<GitResult> ExcecuteASync(string directory, string command)
        {
            var result = await Task.Factory.StartNew(() =>
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

                    return new GitResult()
                    {
                        Output = CheckOutputString(standardOutput, standardError),
                        ExitCode = gitProcess.ExitCode
                    };
                }
                catch (Win32Exception w32ex)
                {
                    throw new Exception(string.Format("Is Git is initialized? {0}", w32ex.Message));
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            });

            return result;
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
