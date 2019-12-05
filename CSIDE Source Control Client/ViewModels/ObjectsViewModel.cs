using CSIDESourceControl.Client.Commands;
using CSIDESourceControl.Git;
using CSIDESourceControl.Client.Service;
using CSIDESourceControl.Models;
using CSIDESourceControl.ObjectHandling;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CSIDESourceControl.ExportFinexe;
using CSIDESourceControl.Client.Helpers;
using CSIDESourceControl.Helpers;
using System.Threading;

namespace CSIDESourceControl.Client.ViewModels
{
    public class ObjectsViewModel : INotifyPropertyChanged
    {
        private IObjectsViewDialogService _dialogService;
        private ObservableCollection<NavObjectModel> _navObjects;
        private string _destinationFolder;
        private string _gitOutput;
        private string _gitRemoteUrl;
        private string _gitCommitMessage;
        private bool _isWorking;
        private int _progressBarValue;
        private string _progressBarStatusText;

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
        private RelayCommand<string> _importFinexeFile;

        public ObjectsViewModel(IObjectsViewDialogService dialogService)
        {
            _dialogService = dialogService;
            _dialogService.TimerElapsed += _dialogService_TimerElapsed;
            _navObjects = new ObservableCollection<NavObjectModel>();

            LoadRecentFolder();
        }

        public ObservableCollection<NavObjectModel> NavObjects
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

        public bool IsWorking
        {
            get { return _isWorking; }
            set { _isWorking = value; OnPropertyChange("IsWorking"); }
        }

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set { _progressBarValue = value; OnPropertyChange("ProgressBarValue"); }
        }

        public string ProgressBarStatusText
        {
            get { return _progressBarStatusText; }
            set { _progressBarStatusText = value; OnPropertyChange("ProgressBarStatusText"); }
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

        public ICommand ImportFinexeFileCommand
        {
            get
            {
                if (_importFinexeFile == null)
                    _importFinexeFile = new RelayCommand<string>(param => ImportFinExeFile(), param => true);

                return _importFinexeFile;
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
            SaveRecentFolder();
        }

        private void SaveRecentFolder()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                RecentDestinationFolder.Save(config, DestinationFolder);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Config Save File Error", ex.Message);
            }
        }

        private void LoadRecentFolder()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                DestinationFolder = RecentDestinationFolder.Read(config);

                // LoadFiles();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Config Read File Error", ex.Message);
            }
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

                    GitOutput = string.Format("Import success");

                    // Removes everything and adds new objects
                    NavObjects = new ObservableCollection<NavObjectModel>(import.NavObjects.Values);
                }

                // Export to new destination
                ObjectsExport.ExportObjects(NavObjects.ToList<NavObjectModel>(), DestinationFolder);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Error importing Object File", ex.Message);
            }
        }

        private async void ImportFinExeFile()
        {
            SettingsHelper settingHelper = new SettingsHelper(DestinationFolder);
            ExportFilterModel exportFilter = settingHelper.ReadFilterSettings();

            if (exportFilter == null)
                exportFilter = new ExportFilterModel();

            SettingsHelper reader = new SettingsHelper(DestinationFolder);
            ServerSetupModel serverSetup = reader.ReadServerSettings();

            if (_dialogService.ImportFromFinExe(ref exportFilter))
            {
                await Export(settingHelper, exportFilter, serverSetup);
            }
        }

        private async Task Export(SettingsHelper settingHelper, ExportFilterModel exportFilter, ServerSetupModel serverSetup)
        {
            ExportFinexeHelper exportFinExe = new ExportFinexeHelper();
            exportFinExe.OnExportError += ExportFinExe_OnExportError;

            StartWorking("Importing");

            var result = await exportFinExe.ExportObjectsFromFinExe(serverSetup, exportFilter);

            if (result.Success)
            {
                string[] importFiles = new string[] { result.ExportedObjectsPath };
                ImportFiles(importFiles);

                // Save Config
                settingHelper.SerializeToSettingsFile(serverSetup, exportFilter);
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Message))
                    _dialogService.ShowErrorMessage("Import Object Files Error", result.Message);
            }

            StopWorking();
        }

        private void ExportFinExe_OnExportError(object source, ExportErrorEventArgs e)
        {
            IsWorking = false;
            _dialogService.ShowErrorMessage("Objects from C/SIDE Export Error", e.Exception.Message);
        }

        public void GitCommit()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                IsWorking = true;

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
            finally
            {
                IsWorking = false;
            }
        }

        public void GitInit()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                IsWorking = true;

                // Settings
                SettingsHelper settingHelper = new SettingsHelper(DestinationFolder); 
                if(!settingHelper.SettingsFolderExists())
                    settingHelper.SerializeToSettingsFile(new ServerSetupModel(), new ExportFilterModel());

                // .gitignore
                GitHelper.CreateGitFiles(DestinationFolder);

                GitProcess.Excecute(DestinationFolder, "init", out string output);

                CheckGitOutput(output);
                GitOutput = output;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Init Error", ex.Message);
            }
            finally
            {
                IsWorking = false;
            }
        }

        public void GitStatus()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                IsWorking = true;

                GitGetRemote();

                GitProcess.Excecute(DestinationFolder, "status", out string output);

                CheckGitOutput(output);
                GitOutput = output;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
            }
            finally
            {
                IsWorking = false;
            }
        }

        public async Task GitSync()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                GitGetRemote();
                GitOutput = "Start sync...";

                StartWorking("Sync");

                GitResult pullResult = await GitProcess.ExcecuteASync(DestinationFolder, "pull origin master");
                if (pullResult.ExitCode != 0)
                    GitOutput = pullResult.Output;

                GitResult pushResult = await GitProcess.ExcecuteASync(DestinationFolder, "push");
                if (pushResult.ExitCode == 0)
                    GitOutput = string.Format("Succesfully pulled and pushed from server\n{0}", pushResult.Output);
                else
                    GitOutput = pushResult.Output;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
            }
            finally
            {
                StopWorking();
            }
        }

        public void GitPush()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                StartWorking("Push");

                GitGetRemote();

                GitProcess.Excecute(DestinationFolder, "push --set-upstream origin master", out string output);
                GitOutput = output;

            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Push Error", ex.Message);
            }
            finally
            {
                StopWorking();
            }
        }

        public void GitPull()
        {
            if (!IsDestinationFolderSet())
                return;

            try
            {
                IsWorking = true;

                GitGetRemote();

                GitProcess.Excecute(DestinationFolder, "pull origin master", out string output);
                GitOutput = output;

                GitStatus();
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Pull Error", ex.Message);
            }
            finally
            {
                IsWorking = false;
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
                IsWorking = true;

                GitProcess.Excecute(DestinationFolder, string.Format(@"config --get remote.origin.url"), out string output);
                GitRemoteUrl = Regex.Replace(output, @"\t|\n|\r", string.Empty);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Status Error", ex.Message);
            }
            {
                IsWorking = false;
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

        private void _dialogService_TimerElapsed(int value)
        {
            ProgressBarValue = value;
        }

        private void StartWorking(string what)
        {
            IsWorking = true;
            ProgressBarValue = 0;
            ProgressBarStatusText = what;
            _dialogService.StartTimer();
        }

        private void StopWorking()
        {
            IsWorking = false;
            ProgressBarValue = 100;
            ProgressBarStatusText = "Done";
            _dialogService.StopTimer();

            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    Thread.Sleep(3000);
                    ProgressBarValue = 0;
                    ProgressBarStatusText = string.Empty;

                    GitStatus();
                }
            ));

            backgroundThread.Start();
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
