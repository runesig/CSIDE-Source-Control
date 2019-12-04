using CSIDESourceControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSIDESourceControl.Client.ViewModels
{
    public class ServerSetupViewModel : INotifyPropertyChanged
    {
        private ServerSetupModel _serverSetup;

        public ServerSetupViewModel(ServerSetupModel serverSetup)
        {
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
            set { _serverSetup.UseNTAuthentication = value; OnPropertyChange("UseNTAuthentication"); }
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
