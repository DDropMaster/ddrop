using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDrop.Models
{
    public class UserView : BindableObject
    {
        private string _email;

        private string _firstName;

        private bool _isAnySelectedSeriesCantDrawPlot;

        private bool _isLoggedIn;

        private string _lastName;

        private string _password;

        private byte[] _userPhoto;

        private ObservableCollection<SeriesView> _userSeries;

        public UserView()
        {
            _userSeries = new ObservableCollection<SeriesView>();
            _userSeries.CollectionChanged += _userSeries_CollectionChanged;
        }

        public Guid UserId { get; set; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                RaisePropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                RaisePropertyChanged("LastName");
            }
        }

        public byte[] UserPhoto
        {
            get => _userPhoto;
            set
            {
                _userPhoto = value;
                RaisePropertyChanged("UserPhoto");
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

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RaisePropertyChanged("Email");
            }
        }

        public ObservableCollection<SeriesView> UserSeries
        {
            get => _userSeries;
            set
            {
                _userSeries = value;
                RaisePropertyChanged("UserSeries");

                foreach (var userSeries in _userSeries)
                {
                    userSeries.PropertyChanged += SeriesPropertyChanged;
                }
            }
        }

        void SeriesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("UserSeries");
        }

        [NotMapped]
        public bool IsAnySelectedSeriesCanDrawPlot
        {
            get
            {
                var checkedSeries = _userSeries?.Where(x => x.IsChecked).ToList();

                if (checkedSeries?.Count != 0)
                {
                    var isAnyCheckedCantDrawPlot = checkedSeries?.Where(x => x.CanDrawPlot != true).ToList().Count > 0;

                    if (isAnyCheckedCantDrawPlot)
                        return false;

                    return true;
                }

                return false;
            }
            set
            {
                _isAnySelectedSeriesCantDrawPlot = value;
                RaisePropertyChanged("IsAnySelectedSeriesCantDrawPlot");
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                RaisePropertyChanged("IsLoggedIn");
            }
        }

        private ObservableCollection<PlotView> _plots;
        public ObservableCollection<PlotView> Plots
        {
            get => _plots;
            set
            {
                _plots = value;
                RaisePropertyChanged("Plots");
            }
        }


        private void _userSeries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsAnySelectedSeriesCanDrawPlot));
        }
    }
}