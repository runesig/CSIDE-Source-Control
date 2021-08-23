using CSIDESourceControl.Client.Commands;
using CSIDESourceControl.Client.Service;
using CSIDESourceControl.Client.Views;
using CSIDESourceControl.ExportFinexe;
using CSIDESourceControl.Helpers;
using CSIDESourceControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSIDESourceControl.Client.ViewModels
{
    public class ImportViewModel : INotifyPropertyChanged
    {
        private RelayCommand<object> _showServerSetupDialog;
        private readonly ExportFilterModel _exportFilter;
        private readonly string _destinationFolder;

        public ImportViewModel(ExportFilterModel exportFilter, string destinationFolder)
        {
            _exportFilter = exportFilter;
            _destinationFolder = destinationFolder;

            CreateFilter();
        }

        public ICommand ServerSetupDialogCommand
        {
            get
            {
                if (_showServerSetupDialog == null)
                    { _showServerSetupDialog = new RelayCommand<object>(param => ShowServerSetupDialog(), param => true); }
                
                return _showServerSetupDialog;
            }
        }

        public void ShowServerSetupDialog()
        {
            SettingsHelper settingsHelper = new SettingsHelper(_destinationFolder);
            ServerSetupModel currentServerSetup = settingsHelper.ReadServerSettings();
            ExportFilterModel exportFilter = settingsHelper.ReadFilterSettings();
            GitSettingsModel gitSettingsModel = settingsHelper.ReadGitSettings();

            // TODO: not part of MVVM Start
            ServerSetupDialogService dialogService = new ServerSetupDialogService();
            ServerSetupViewModel viewModel = new ServerSetupViewModel(dialogService, currentServerSetup);

            ServerSetupView view = new ServerSetupView
            {
                DataContext = viewModel
            };
            // TODO: not part of MVVM Stop

            bool? dialogResult = view.ShowDialog();
            currentServerSetup = viewModel.ServerSetup;

            if ((dialogResult.HasValue) && (dialogResult.Value))
                settingsHelper.SerializeToSettingsFile(currentServerSetup, exportFilter, gitSettingsModel);
        }

        public ExportFilterModel GetImportFilters()
        {
            return _exportFilter;
        }
        public bool Modified
        {
            get { return _exportFilter.Modified; }
            set { _exportFilter.Modified = value; CreateFilter(); OnPropertyChange("Modified"); }
        }

        public DateTime? DateFrom
        {
            get { return _exportFilter.DateFrom; }
            set { _exportFilter.DateFrom = value; CreateFilter(); OnPropertyChange("DateFrom"); }
        }

        public DateTime? DateTo
        {
            get { return _exportFilter.DateTo; }
            set { _exportFilter.DateTo = value; CreateFilter(); OnPropertyChange("DateTo"); }
        }

        public string VersionList
        {
            get { return _exportFilter.VersionList; }
            set { _exportFilter.VersionList = value; CreateFilter(); OnPropertyChange("VersionList"); }
        }

        public bool UseCustomFilter
        {
            get { return _exportFilter.UseCustomFilter; }
            set { _exportFilter.UseCustomFilter = value; OnPropertyChange("UseCustomFilter"); }
        }

        public string CustomFilter
        {
            get { return _exportFilter.CustomFilter; }
            set { _exportFilter.CustomFilter = value; OnPropertyChange("CustomFilter"); }
        }

        private void CreateFilter()
        {
            CustomFilter = ExportFilter.CreateFilterString(
                Modified,
                DateFrom,
                DateTo,
                VersionList,
                UseCustomFilter,
                CustomFilter);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
