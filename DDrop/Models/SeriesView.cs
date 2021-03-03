using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace DDrop.Models
{
    public class SeriesView : INotifyPropertyChanged
    {
        private string _addedDate;

        private bool _canDrawPlot;

        private UserView _currentUser;
        private Guid _currentUserId;

        private ObservableCollection<MeasurementView> _measurementsSeries;

        private bool _exactCalculationModel;

        private double _intervalBetweenPhotos;

        private bool _isChecked;

        private bool _loaded = true;

        private ReferencePhotoView _referencePhotoForSeries;

        private SubstanceModelView _substance;

        private string _title;

        public SeriesView()
        {
            _measurementsSeries = new ObservableCollection<MeasurementView>();
            _measurementsSeries.CollectionChanged += _measurementsSeries_CollectionChanged;
        }

        public Guid CurrentUserId
        {
            get => _currentUserId;
            set
            {
                _currentUserId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUserId"));
            }
        }

        public UserView CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentUser"));
            }
        }

        public Guid SeriesId { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Title"));
            }
        }

        public ObservableCollection<MeasurementView> MeasurementsSeries
        {
            get => _measurementsSeries;
            set
            {
                _measurementsSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MeasurementsSeries"));
            }
        }

        public ReferencePhotoView ReferencePhotoForSeries
        {
            get => _referencePhotoForSeries;
            set
            {
                _referencePhotoForSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReferencePhotoForSeries"));
            }
        }

        private object _sub;
        public object Sub
        {
            get => _sub;
            set
            {
                _sub = value;
                if (_sub != null)
                {
                    var json = JsonConvert.SerializeObject(value);
                    var keyValuePair = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(json);
                    Substance = new SubstanceModelView()
                    {
                        Id = keyValuePair.Key,
                        CommonName = keyValuePair.Value,
                        SubstanceId = SeriesId
                    };

                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Substance)));
                }

                OnPropertyChanged(new PropertyChangedEventArgs("Sub"));
            }
        }

        public SubstanceModelView Substance
        {
            get => _substance;
            set
            {
                _substance = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Substance"));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsSubstanceExists)));
            }
        }

        public bool ExactCalculationModel
        {
            get => _exactCalculationModel;
            set
            {
                _exactCalculationModel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ExactCalculationModel"));
            }
        }

        public double IntervalBetweenPhotos
        {
            get => _intervalBetweenPhotos;
            set
            {
                _intervalBetweenPhotos = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IntervalBetweenPhotos"));
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }

        public bool CanDrawPlot
        {
            get
            {
                if (_measurementsSeries.Count > 0 && _measurementsSeries.All(x => x.Processed))
                    return true;

                return false;
            }
            set
            {
                _canDrawPlot = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanDrawPlot"));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawAnyPlot)));
            }
        }

        private bool _canDrawTemperaturePlot;
        public bool CanDrawTemperaturePlot
        {
            get
            {
                if (_measurementsSeries.Count > 0 && _measurementsSeries.All(x => x.ProcessedThermal))
                {
                    return true;
                }

                return false;
            }
            set
            {
                _canDrawTemperaturePlot = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanDrawTemperaturePlot"));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawAnyPlot)));
            }
        }

        private bool _canDrawAnyPlot;
        public bool CanDrawAnyPlot
        {
            get
            {
                if (CanDrawPlot || CanDrawTemperaturePlot)
                {
                    return true;
                }

                return false;
            }
            set
            {
                _canDrawAnyPlot = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CanDrawAnyPlot"));
            }
        }

        private bool _isSubstanceEdited;
        public bool IsSubstanceEdited
        {
            get => _isSubstanceEdited;
            set
            {
                _isSubstanceEdited = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsSubstanceEdited"));
            }
        }

        private bool _isSubstanceExists;
        public bool IsSubstanceExists
        {
            get
            {
                if (_substance != null)
                {
                    return true;
                }

                return false;
            }
            set
            {
                _isSubstanceExists = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsSubstanceExists"));
            }
        }

        public bool Loaded
        {
            get => _loaded;
            set
            {
                _loaded = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Loaded"));
            }
        }

        public string AddedDate
        {
            get => _addedDate;
            set
            {
                _addedDate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddedDate"));
            }
        }

        private Guid? _commentId;
        public Guid? CommentId
        {
            get => _commentId;
            set
            {
                _commentId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CommentId"));
            }
        }

        private CommentView _comment;
        public CommentView Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Comment"));
            }
        }

        private SeriesSettingsView _settings;
        public SeriesSettingsView Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Settings"));
            }
        }

        private ObservableCollection<TypedRectangleView> _regionOfInterest;
        public ObservableCollection<TypedRectangleView> RegionOfInterest
        {
            get => _regionOfInterest;
            set
            {
                _regionOfInterest = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RegionOfInterest"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void _measurementsSeries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));
            foreach (var photo in _measurementsSeries) photo.MeasurementOrderInSeries = _measurementsSeries.IndexOf(photo);
            CurrentUser.OnPropertyChanged(new PropertyChangedEventArgs(nameof(UserView.IsAnySelectedSeriesCanDrawPlot)));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Loaded))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(IntervalBetweenPhotos))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));

            if (e.PropertyName == nameof(Settings.GeneralSeriesSettings.UseCreationDateTime))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanDrawPlot)));
            
            if (e.PropertyName == nameof(IsChecked))
                CurrentUser.OnPropertyChanged(
                    new PropertyChangedEventArgs(nameof(UserView.IsAnySelectedSeriesCanDrawPlot)));

            PropertyChanged?.Invoke(this, e);
        }
    }
}