using CSIDESourceControl.Client.ViewModels;
using CSIDESourceControl.Client.Views;
using CSIDESourceControl.Models;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;

namespace CSIDESourceControl.Client.Service
{
    public class ObjectsViewDialogService : IObjectsViewDialogService
    {
        private Timer _timer;
        private int _progressValue;

        public event TimerElapsedEventHandler TimerElapsed;

        public ObjectsViewDialogService()
        {
            _progressValue = 0;
            _timer = new Timer(Timer_Elapsed, null, 0, Timeout.Infinite);
        }

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

        public bool SetRemote(out string remoteurl)
        {
            RemoteViewModel viewModel = new RemoteViewModel();

            RemoteView remote = new RemoteView();
            remote.DataContext = viewModel;

            bool? dialogResult = remote.ShowDialog();
            remoteurl = viewModel.RemoteUrl;

            if (dialogResult.HasValue)
                return dialogResult.Value;

            return false;
        }

        public bool ImportFromFinExe(ref ExportFilterModel importSettings)
        {
            ImportViewModel viewModel = new ImportViewModel(importSettings);

            ImportView import = new ImportView();
            import.DataContext = viewModel;

            bool? dialogResult = import.ShowDialog();

            if (dialogResult.HasValue)
                return dialogResult.Value;

            return false;
        }

        public void StartTimer()
        {
            _progressValue = 0;
            _timer.Change(1000, 1000);
        }

        public void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void Timer_Elapsed(object state)
        {
            _progressValue += 1;
            if (_progressValue >= 100)
                _progressValue = 100;

            TimerElapsed?.Invoke(_progressValue);
        }
    }
}
