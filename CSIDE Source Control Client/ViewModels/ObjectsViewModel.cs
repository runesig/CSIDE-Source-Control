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
        private readonly IObjectsViewDialogService _dialogService;
        private ObservableCollection<NavObjectModel> _navObjects;
        private string _destinationFolder;
        private string _gitOutput;
        private string _branch;
        private string _gitRemoteUrl;
        private string _gitCommitMessage;
        private bool _isWorking;
        private int _progressBarValue;
        private string _progressBarStatusText;

        private RelayCommand<object> _showOpenFileDialog;
        private RelayCommand<object> _showSelectDestinationFolder;
        private RelayCommand<object> _showServerSettings;
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
            _dialogService.TimerElapsed += DialogService_TimerElapsed;
            _navObjects = new ObservableCollection<NavObjectModel>();

            LoadRecentFolder();
            LoadFromDestinationFolder();
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
                    _gitSync = new RelayCommand<object>(async(param) => await GitSync(), param => true);
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

        public ICommand ShowServerSettings
        {
            get
            {
                if (_showServerSettings == null)
                {
                    _showServerSettings = new RelayCommand<object>(param => ServerSettings(), param => true);
                }
                return _showServerSettings;
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
            if (_dialogService.GetFolder("Select Destination Folder", out string path))
                DestinationFolder = path;

            GitGetRemote();
            SaveRecentFolder();
            LoadFromDestinationFolder();
        }

        private void ServerSettings()
        {
            if (!IsDestinationFolderSet())
                return;

            SettingsHelper settingsHelper = new SettingsHelper(DestinationFolder);
            ServerSetupModel currentServerSetup = settingsHelper.ReadServerSettings();
            ExportFilterModel exportFilter = settingsHelper.ReadFilterSettings();
            GitSettingsModel gitSettingsModel = settingsHelper.ReadGitSettings();

            if (_dialogService.ShowServerSettings(ref currentServerSetup))
            {
                settingsHelper.SerializeToSettingsFile(currentServerSetup, exportFilter, gitSettingsModel);
            }
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
                    // First import the "big" object file
                    string filePath = filePaths[0];
                    ObjectsImport objectFileImport = new ObjectsImport();
                    objectFileImport.RunImportFromObjectFile(filePath);
                    objectFileImport.CleanUpRemovedFiles(DestinationFolder);

                    // Export in folders to new destination
                    ObjectsExport.ExportObjects(objectFileImport.GetObjectList(), DestinationFolder);

                    // Now reimport from new destination to get all files
                    LoadFromDestinationFolder();

                    GitOutput = string.Format("Import success");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Error importing Object File", ex.Message);
            }
        }

        private async void ImportFinExeFile()
        {
            if (!IsDestinationFolderSet())
                return;

            SettingsHelper settingHelper = new SettingsHelper(DestinationFolder);
            ExportFilterModel exportFilter = settingHelper.ReadFilterSettings();
            GitSettingsModel gitSettingsModel = settingHelper.ReadGitSettings();

            SettingsHelper reader = new SettingsHelper(DestinationFolder);
            ServerSetupModel serverSetup = reader.ReadServerSettings();

            if (_dialogService.ImportFromFinExe(DestinationFolder, ref exportFilter))
            {
                await Export(settingHelper, exportFilter, serverSetup, gitSettingsModel);
            }
        }

        private async Task Export(SettingsHelper settingHelper, ExportFilterModel exportFilter, ServerSetupModel serverSetup, GitSettingsModel gitSettingsModel)
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
                settingHelper.SerializeToSettingsFile(serverSetup, exportFilter, gitSettingsModel);
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
                SetModifiedFiles(new List<string>()); // Clear
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
                    settingHelper.SerializeToSettingsFile(new ServerSetupModel(), new ExportFilterModel(), new GitSettingsModel());

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

                List<string> modifiedFiles = GitProcess.CheckModifiedFilesFromOutput(output);
                SetModifiedFiles(modifiedFiles);

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
                SettingsHelper settingHelper = new SettingsHelper(DestinationFolder);
                GitSettingsModel gitSettingsModel = settingHelper.ReadGitSettings();

                GitGetRemote();
                GitOutput = "Start sync...";

                StartWorking("Sync");

                GitResult pullResult = await GitProcess.ExcecuteASync(DestinationFolder, string.Format("pull origin {0}", gitSettingsModel.Branch));
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
                SettingsHelper settingHelper = new SettingsHelper(DestinationFolder);
                GitSettingsModel gitSettingsModel = settingHelper.ReadGitSettings();

                StartWorking("Push");

                GitGetRemote();

                GitProcess.Excecute(DestinationFolder, string.Format("push --set-upstream origin {0}", gitSettingsModel.Branch), out string output);

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
                SettingsHelper settingHelper = new SettingsHelper(DestinationFolder);
                GitSettingsModel gitSettingsModel = settingHelper.ReadGitSettings();

                IsWorking = true;

                GitGetRemote();

                GitProcess.Excecute(DestinationFolder, string.Format("pull origin {0}", gitSettingsModel.Branch), out string output);
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
            finally
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

        private void DialogService_TimerElapsed(int value)
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

        private void LoadFromDestinationFolder()
        {
            try
            {
                if (string.IsNullOrEmpty(DestinationFolder))
                    return;

                ObjectsImport import = new ObjectsImport();
                import.RunImportFromDestinationFolder(DestinationFolder);

                // Removes everything and adds new objects to collection for viewing in appliaction
                NavObjects = new ObservableCollection<NavObjectModel>(import.NavObjects.Values);
            }
            catch (Exception ex)
            {
                DestinationFolder = string.Empty;

                _dialogService.ShowErrorMessage("Destination Folder Error", ex.Message);
            }
        }

        private void SetModifiedFiles(List<string> modifiedFiles)
        {
            foreach (NavObjectModel navObject in NavObjects)
            {
                navObject.IsEdited = false;

                foreach (string modifiedFile in modifiedFiles)
                {
                    if (navObject.Type.ToUpper() == NavObjectModel.PathToType(modifiedFile))
                        navObject.IsEdited = true;

                    if (navObject.InternalId.ToUpper() == NavObjectModel.FilePathToInternalId(modifiedFile))
                        navObject.IsEdited = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
