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
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSIDESourceControl.Client.ViewModels
{
    public class ObjectsViewModel : INotifyPropertyChanged
    {
        private IDialogService _dialogService;
        private ObservableCollection<NavObject> _navObjects;
        private string _destinationFolder;

        private RelayCommand<object> _showOpenFileDialog;
        private RelayCommand<object> _showSelectDestinationFolder;
        private RelayCommand<object> _gitCommit;
        private RelayCommand<string[]> _importFiles;

        public ObjectsViewModel(IDialogService dialogService)
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
        }

        private void ImportFiles(string[] filePaths)
        {
            if (filePaths == null)
                return;

            if (string.IsNullOrEmpty(DestinationFolder))
                SelectDestinationFolder();

            try
            {
                if (string.IsNullOrEmpty(DestinationFolder))
                    throw new Exception("No destination folder is selected");

                if (filePaths.Length > 0)
                {
                    ObjectsImport import = new ObjectsImport();
                    import.RunImport(filePaths[0]);

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
            try 
            {
                string output = GitProcess.Excecute(DestinationFolder, "init");

                _dialogService.ShowInformationMessage("Git", output);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorMessage("Git Command Error", ex.Message);
            }
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
