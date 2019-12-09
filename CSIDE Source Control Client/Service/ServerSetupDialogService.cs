using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSIDESourceControl.Client.Service
{
    public class ServerSetupDialogService : IServerSetupDialogService
    {
        public bool OpenFinExeFile(ref string finExePath)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Title = "Open finsql.exe",
                    Filter = "EXE files|*.exe",
                    Multiselect = false
                };

                Nullable<bool> result = openDialog.ShowDialog();

                if (result == true)
                {
                    if (openDialog.FileNames.Length > 0)
                    {
                        finExePath = openDialog.FileName;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message);
            }

            return false;
        }

        public void ShowErrorMessage(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
