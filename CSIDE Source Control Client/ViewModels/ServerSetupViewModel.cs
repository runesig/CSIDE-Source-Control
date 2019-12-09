using CSIDESourceControl.Client.Commands;
using CSIDESourceControl.Client.Service;
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
    public class ServerSetupViewModel : INotifyPropertyChanged
    {
        private RelayCommand<object> _showOpenFinExeDialog;

        private IServerSetupDialogService _dialogService;
        private ServerSetupModel _serverSetup;
        private bool _enableEditCredentials;

        public ServerSetupViewModel(IServerSetupDialogService dialogService, ServerSetupModel serverSetup)
        {
            _dialogService = dialogService;
            _serverSetup = serverSetup;
        }

        public ServerSetupModel ServerSetup
        {
            get { return _serverSetup; }
        }

        public string FinExePath
        {
            get { return _serverSetup.FinExePath; }
            set { _serverSetup.FinExePath = value; OnPropertyChange("FinExePath"); }
        }

        public string Server
        {
            get { return _serverSetup.Server; }
            set { _serverSetup.Server = value; OnPropertyChange("Server"); }
        }

        public string Database
        {
            get { return _serverSetup.Database; }
            set { _serverSetup.Database = value; OnPropertyChange("Database"); }
        }

        public bool UseNTAuthentication
        {
            get { return _serverSetup.UseNTAuthentication; }
            set { _serverSetup.UseNTAuthentication = value; EnableEditCredentials = !value;  OnPropertyChange("UseNTAuthentication"); }
        }

        public string UserName
        {
            get { return _serverSetup.UserName; }
            set { _serverSetup.UserName = value; OnPropertyChange("UserName"); }
        }

        public string Password
        {
            get { return _serverSetup.Password; }
            set { _serverSetup.Password = value; OnPropertyChange("Password"); }
        }

        public bool EnableEditCredentials
        {
            get { return _enableEditCredentials; }
            set { _enableEditCredentials = value; OnPropertyChange("EnableEditCredentials"); }
        }

        public ICommand ShowOpenFinExeDialog
        {
            get
            {
                if (_showOpenFinExeDialog == null)
                {
                    _showOpenFinExeDialog = new RelayCommand<object>(param => OpenFinExeDialog(), param => true);
                }
                return _showOpenFinExeDialog;
            }
        }

        private void OpenFinExeDialog()
        {
            string finExePath = string.Empty;

            if (_dialogService.OpenFinExeFile(ref finExePath))
            {
                if (!string.IsNullOrEmpty(finExePath))
                    FinExePath = finExePath;
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
