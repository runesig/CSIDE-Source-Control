using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CSIDESourceControl.Client.Commands;
using CSIDESourceControl;
using CSIDESourceControl.Client.Service;

namespace CSIDESourceControlClient.ViewModels
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private User _user;
        private IObjectsViewDialogService _messageBoxService;

        public MyViewModel(IObjectsViewDialogService messageBoxService)
        {
            _messageBoxService = messageBoxService;

            _user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = DateTime.Now.AddYears(-30)
            };
        }

        // Commands Start
        private RelayCommand<object> _commandStart;
 
        public ICommand CmdStartExecution
        {
            get
            {
                if (_commandStart == null)
                {
                    // _commandStart = new RelayCommand<Action>(param => Start(), param => CanStart());
                    _commandStart = new RelayCommand<object>(param => _messageBoxService.ShowInformationMessage("Crap", "Shait"));
                }
                return _commandStart;
            }
        }

        public void Start()
        {
            //Do what ever
            
        }

        public bool CanStart()
        {
            return (DateTime.Now.DayOfWeek == DayOfWeek.Monday); //Can only click that button on mondays.
        }
        // Commands Stop



        public string FirstName
        {
            get { return _user.FirstName; }
            set
            {
                if (_user.FirstName != value)
                {
                    _user.FirstName = value;
                    OnPropertyChange("FirstName");
                    // If the first name has changed, the FullName property needs to be udpated as well.
                    OnPropertyChange("FullName");
                }
            }
        }

        public string LastName
        {
            get { return _user.LastName; }
            set
            {
                if (_user.LastName != value)
                {
                    _user.LastName = value;
                    OnPropertyChange("LastName");
                    // If the first name has changed, the FullName property needs to be udpated as well.
                    OnPropertyChange("FullName");
                }
            }
        }

        // This property is an example of how model properties can be presented differently to the View.
        // In this case, we transform the birth date to the user's age, which is read only.
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - _user.BirthDate.Year;
                if (_user.BirthDate > today.AddYears(-age)) age--;
                return age;
            }
        }

        // This property is just for display purposes and is a composition of existing data.
        public string FullName
        {
            get { return FirstName + " " + LastName; }
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
