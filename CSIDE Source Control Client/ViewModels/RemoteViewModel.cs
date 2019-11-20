using CSIDESourceControl.Client.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSIDESourceControl.Client.ViewModels
{
    public class RemoteViewModel : INotifyPropertyChanged
    {
        private string _remoteUrl;

        public string RemoteUrl
        {
            get { return _remoteUrl; }
            set { _remoteUrl = value; OnPropertyChange("RemoteUrl"); }
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
