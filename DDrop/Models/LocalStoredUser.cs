using System.Collections.ObjectModel;

namespace DDrop.Models
{
    public class LocalStoredUser : BindableObject
    {
        private bool _isChecked;
        private string _login;

        private string _password;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                RaisePropertyChanged("Login");
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
    }

    public class LocalStoredUsers : BindableObject
    {
        private ObservableCollection<LocalStoredUser> _users;

        public ObservableCollection<LocalStoredUser> Users
        {
            get => _users;
            set
            {
                _users = value;
                RaisePropertyChanged("Users");
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get =>_users?.Count > 0;
            set
            {
                _isEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }
    }
}