using CSIDESourceControl.Client.Commands;
using CSIDESourceControl.Client.Git;
using CSIDESourceControl.Client.Service;
using CSIDESourceControl.Models;
using CSIDESourceControl.ObjectHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSIDESourceControl.Client.ViewModels
{
    public class ObjectsViewModel : INotifyPropertyChanged
    {
        private IObjectsViewDialogService _dialogService;
        private ObservableCollection<NavObject> _navObjects;
        private string _destinationFolder;
        private string _gitOutput;
        private string _gitRemoteUrl;
        private string _gitCommitMessage;

        private RelayCommand<object> _showOpenFileDialog;
        private RelayCommand<object> _showSelectDestinationFolder;
        private RelayCommand<object> _gitCommit;
        private RelayCommand<object> _gitInit;
        private RelayCommand<object> _gitSetremote;
        private RelayCommand<object> _gitStatus;
        private RelayCommand<object> _gitPull;
        private RelayCommand<object> _gitPush;
        private RelayCommand<object> _gitSync;
        private RelayCommand<string[]> _importFiles;

        public ObjectsViewModel(IObjectsViewDialogService dialogService)
        {
            _dialogService = dialogService;
            _navObjects = new ObservableCollection<NavObject>();
        }

        public ObservableCollection<NavObject> NavObjects
        {
            get { return _navObjects; }
            set { _navObjects = value; OnPropertyChange("NavObjects"); }
        }

        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set { _destinationFolder = value; OnPropertyChange("DestinationFolder"); }
        }

        public string GitRemoteUrl
        {
            get { return _gitRemoteUrl; }
            set { _gitRemoteUrl = value; OnPropertyChange("GitRemoteUrl"); }
        }

        public string GitOutput
        {
            get { return _gitOutput; }
            set { _gitOutput = value; OnPropertyChange("GitOutput"); }
        }

        public string GitCommitMessage
        {
            get { return _gitCommitMessage; }
            set { _gitCommitMessage = value; OnPropertyChange("GitCommitMessage"); }
        }
        
        public ICommand GitCommitCommand
        {
            get
            {
                if (_gitCommit == null)
                {
                    _gitCommit = new RelayCommand<object>(param => GitCommit(), param => true);
                }
                return _gitCommit;
            }
        }

        public ICommand GitInitCommand
        {
            get
            {
                if (_gitInit == null)
                {
                    _gitInit = new RelayCommand<object>(param => GitInit(), param => true);
                }
                return _gitInit;
            }
        }

        public ICommand GitSetRemoteCommand
        {
            get
            {
                if (_gitSetremote == null)
                {
                    _gitSetremote = new RelayCommand<object>(param => GitSetRemote(), param => true);
                }
                return _gitSetremote;
            }
        }

        public ICommand GitStatusCommand
        {
            get
            {
                if (_gitStatus == null)
                {
                    _gitStatus = new RelayCommand<object>(param => GitStatus(), param => true);
                }
                return _gitStatus;
            }
        }

        public ICommand GitSyncCommand
        {
            get
            {
                if (_gitSync == null)
                {
                    _gitSync = new RelayCommand<object>(param => GitSync(), param => true);
                }
                return _gitSync;
            }
        }

        public ICommand GitPushCommand
        {
            get
            {
                if (_gitPush == null)
                {
                    _gitPush = new RelayCommand<object>(param => GitPush(), param => true);
                }
                return _gitPush;
            }
        }

        public ICommand GitPullCommand
        {
            get
            {
                if (_gitPull == null)
                {
                    _gitPull = new RelayCommand<object>(param => GitPull(), param => true);
                }
                return _gitPull;
            }
        }

        public ICommand ShowOpenFileDialog
        {
            get
            {
                if (_showOpenFileDialog == null)
                {
                    _showOpenFileDialog = new RelayCommand<object>(param => OpenFileDialog(), param => true);
                }
                return _showOpenFileDialog;
            }
        }

        public ICommand ShowSelectDestinationFolder
        {
            get
            {
                if (_showSelectDestinationFolder == null)
                {
                    _showSelectDestinationFolder = new RelayCommand<object>(param => SelectDestinationFolder(), param => true);
                }
                return _showSelectDestinationFolder;
            }
        }

        public ICommand ImportFilesCommand
        {
            get
            {
                if (_importFiles == null)
                    _importFiles = new RelayCommand<string[]>(param => ImportFiles(param), param => true);

                return _importFiles;
            }
        }

        private void OpenFileDialog()
        {
            string[] filePaths = null;

            _dialogService.OpenFile(ref filePaths);

            // Import the Object File(s)
            ImportFiles(filePaths);
        }

        private void SelectDestinationFolder()
        {
            string path = string.Empty;

            if (_dialogService.GetFolder("Select Destination Folder", out path))
                DestinationFolder = path;

            GitGetRemote();
        }

        private void ImportFiles(string[] filePaths)
        {
            if (filePaths == null)
                return;

            if (!IsDestinationFolderSet())
                return;

            try
            {
                if (filePaths.Length > 0)
                {
                    string filePath = filePaths[0];
                    ObjectsImport import = new ObjectsImport();
                    import.RunImport(filePath);

                    GitOutput = string.Format("Added file: {0}", filePath);

                    // Removes everything and adds new objects
                    NavObjects = new ObservableCollection<NavObject>(import.NavObjects.Values);
                }

                // Export to new destination
                ObjectsExport.ExportObjects(NavObjects.ToList<NavObject>(), DestinationFolder);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Error importing Object File", ex.Message);
            }
        }

        public void GitCommit()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                if (string.IsNullOrEmpty(GitCommitMessage))
                    throw new Exception("Commit message can't be empty.");

                GitProcess.Excecute(DestinationFolder, "add --all", out string output);
                GitOutput = output;

                GitProcess.Excecute(DestinationFolder, string.Format(@"commit -am ""{0}""", GitCommitMessage), out output);
                GitOutput = output;

                GitCommitMessage = string.Empty;

                CheckGitOutput(output);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Commit Error", ex.Message);
            }
        }

        public void GitInit()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                GitProcess.Excecute(DestinationFolder, "init", out string output);

                CheckGitOutput(output);
                GitOutput = output;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Init Error", ex.Message);
            }
        }

        public void GitStatus()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                GitProcess.Excecute(DestinationFolder, "status", out string output);

                CheckGitOutput(output);
                GitOutput = output;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
            }
        }

        public void GitSync()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                GitProcess.Excecute(DestinationFolder, "pull origin master", out string output);
                GitOutput = output;

                if (GitProcess.Excecute(DestinationFolder, "push", out output) == 0)
                    GitOutput = string.Format("Succesfully pulled and pushed from server\n {0}", output);
                else
                    GitOutput = output;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
            }
        }

        public void GitPush()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                GitProcess.Excecute(DestinationFolder, "push --set-upstream origin master", out string output);
                GitOutput = output;

            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Push Error", ex.Message);
            }
        }

        public void GitPull()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                GitProcess.Excecute(DestinationFolder, "pull origin master", out string output);
                GitOutput = output;

                GitStatus();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Pull Error", ex.Message);
            }
        }

        public void GitSetRemote()
        {
            if (!IsDestinationFolderSet())
                return;

            if (_dialogService.SetRemote(out string remoteUrl))
            {
                if (string.IsNullOrEmpty(remoteUrl))
                    return;

                try
                {
                    if(GitProcess.Excecute(DestinationFolder, string.Format(@"remote add origin {0}", remoteUrl), out string output) == 0)
                        GitOutput = string.Format("Succesfully added remote server\n {0}", output);
                    else
                        GitOutput = output;
                    GitGetRemote();
                }
                catch (Exception ex)
                {
                    _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
                }
            }
        }

        private void GitGetRemote()
        {
            try
            {
                GitProcess.Excecute(DestinationFolder, string.Format(@"config --get remote.origin.url"), out string output);
                GitRemoteUrl = Regex.Replace(output, @"\t|\n|\r", string.Empty);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
            }
        }


        private void CheckGitOutput(string output)
        {
            if (string.IsNullOrEmpty(output))
                _dialogService.ShowErrorMessage("Git Command Failed", "Something went wrong. Please check if Git is properly initialized and command is correct.");
        }

        private bool IsDestinationFolderSet()
        {
            if (string.IsNullOrEmpty(DestinationFolder))
                SelectDestinationFolder();

            return !string.IsNullOrEmpty(DestinationFolder);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
