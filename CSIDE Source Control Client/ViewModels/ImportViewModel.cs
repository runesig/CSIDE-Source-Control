using CSIDESourceControl.Client.Commands;
using CSIDESourceControl.Client.Views;
using CSIDESourceControl.ExportFinexe;
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

        private ImportSettings _importSettings;

        public ImportViewModel(ImportSettings importSettings)
        {
            _importSettings = importSettings;

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
            ServerSetup currentServerSetup = new ServerSetup(); // Read from file 

            ServerSetupViewModel viewModel = new ServerSetupViewModel(currentServerSetup);

            ServerSetupView view = new ServerSetupView();
            view.DataContext = viewModel;

            bool? dialogResult = view.ShowDialog();
            ServerSetup newServerSetup = viewModel.ServerSetup;

            if ((dialogResult.HasValue) && (dialogResult.Value))
            { 
                // Write new Server Setup to file
            }
        }

        public ImportSettings GetImportSettings()
        {
            return _importSettings;
        }
        public bool Modified
        {
            get { return _importSettings.Modified; }
            set { _importSettings.Modified = value; CreateFilter(); OnPropertyChange("Modified"); }
        }

        public DateTime? DateFrom
        {
            get { return _importSettings.DateFrom; }
            set { _importSettings.DateFrom = value; CreateFilter(); OnPropertyChange("DateFrom"); }
        }

        public DateTime? DateTo
        {
            get { return _importSettings.DateTo; }
            set { _importSettings.DateTo = value; CreateFilter(); OnPropertyChange("DateTo"); }
        }

        public string VersionList
        {
            get { return _importSettings.VersionList; }
            set { _importSettings.VersionList = value; CreateFilter(); OnPropertyChange("VersionList"); }
        }

        public bool CustomFilter
        {
            get { return _importSettings.CustomFilter; }
            set { _importSettings.CustomFilter = value; OnPropertyChange("CustomFilter"); }
        }

        public string Filter
        {
            get { return _importSettings.Filter; }
            set { _importSettings.Filter = value; OnPropertyChange("Filter"); }
        }

        private void CreateFilter()
        {
            Filter = ExportFilter.Create(
                Modified,
                DateFrom,
                DateTo,
                VersionList,
                CustomFilter,
                Filter);
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
