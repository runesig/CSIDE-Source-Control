using CSIDESourceControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Client.Service
{
    public delegate void TimerElapsedEventHandler(int value);

    public interface IObjectsViewDialogService
    {
        event TimerElapsedEventHandler TimerElapsed;
        void ShowInformationMessage(string caption, string text);
        void ShowErrorMessage(string caption, string text);
        bool OpenFile(ref string[] filePaths);
        bool GetFolder(string description, out string folderPath);
        bool SetRemote(out string remoteUrl);
        bool ImportFromFinExe(ref ExportFilterModel importSettings);
        void StartTimer();
        void StopTimer();
    }
}
