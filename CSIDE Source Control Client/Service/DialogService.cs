using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSIDESourceControl.Client.Service
{
    public class DialogService : IDialogService
    {
        public void ShowInformationMessage(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowErrorMessage(string caption, string text)
        {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool OpenFile(ref string[] filePaths)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Title = "Open NAV Object File",
                    Filter = "Txt files|*.txt",
                    Multiselect = false
                };

                Nullable<bool> result = openDialog.ShowDialog();

                if (result == true)
                {
                    if (openDialog.FileNames.Length > 0)
                    {
                        filePaths = openDialog.FileNames;
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

        public bool GetFolder(string description, out string folderPath)
        {
            folderPath = string.Empty;

            try
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = description;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    folderPath = dialog.SelectedPath;

                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message);
            }

            return false;
        }
    }
}
