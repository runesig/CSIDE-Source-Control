using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Client.Service
{
    public interface IServerSetupDialogService
    {
        bool OpenFinExeFile(ref string finExePath);
        void ShowErrorMessage(string caption, string text);
    }
}
