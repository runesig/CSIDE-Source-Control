using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Client.Service
{
    public interface IDialogService
    {
        void ShowInformationMessage(string caption, string text);
        void ShowErrorMessage(string caption, string text);
        bool OpenFile(ref string[] filePaths);

        bool GetFolder(string description, out string folderPath);
    }
}
