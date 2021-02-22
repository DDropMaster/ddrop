using System;
using System.ComponentModel;

namespace DDrop.Models
{
    public class MeasurementView : INotifyPropertyChanged
    {
        private string _addedDate;

        private string _creationDateTime;

        private SeriesView _currentSeries;
        private Guid _currentSeriesId;

        private bool _isChecked;

        private string _name;

        private int _measurementOrderInSeries;

        private bool _requireSaving;

        public Guid CurrentSeriesId
        {
            get => _currentSeriesId;
            set
            {
                _currentSeriesId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSeriesId"));
            }
        }

        public SeriesView CurrentSeries
        {
            get => _currentSeries;
            set
            {
                _currentSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSeries"));
            }
        }

        public Guid MeasurementId { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }

        private DropView _drop;
        public DropView Drop
        {
            get => _drop;
            set
            {
                _drop = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Drop"));
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

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
            }
        }

        public string CreationDateTime
        {
            get => _creationDateTime;
            set
            {
                _creationDateTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CreationDateTime"));
            }
        }

        public bool RequireSaving
        {
            get => _requireSaving;
            set
            {
                _requireSaving = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RequireSaving"));
            }
        }

        public int MeasurementOrderInSeries
        {
            get => _measurementOrderInSeries;
            set
            {
                _measurementOrderInSeries = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MeasurementOrderInSeries"));
            }
        }

        private bool _processed;
        public bool Processed
        {
            get
            {
                if (_drop.RadiusInMeters > 0)
                {
                    if (_frontDropPhoto == null && _sideDropPhoto == null)
                        return false;

                    if (_frontDropPhoto == null && _sideDropPhoto != null && _sideDropPhoto.Processed)
                        return true;

                    if (_sideDropPhoto == null && _frontDropPhoto != null && _frontDropPhoto.Processed)
                        return true;

                    if (_sideDropPhoto == null && _frontDropPhoto != null && !_frontDropPhoto.Processed)
                        return false;

                    if (_frontDropPhoto == null && _sideDropPhoto != null && !_sideDropPhoto.Processed)
                        return false;

                    bool twoPhotos = _frontDropPhoto.PhotoId != Guid.Empty && _sideDropPhoto.PhotoId != Guid.Empty;

                    if (twoPhotos && _frontDropPhoto.Processed && _sideDropPhoto.Processed)
                        return true;

                    if (twoPhotos && !_frontDropPhoto.Processed || !_sideDropPhoto.Processed)
                        return false;

                    if (!twoPhotos && _frontDropPhoto.Processed || _sideDropPhoto.Processed)
                        return true;
                }

                return false;
            }
        }

        private bool _processedThermal;
        public bool ProcessedThermal
        {
            get
            {
                if (Drop.Temperature != null && AmbientTemperature != 0)
                    return true;

                return false;
            }
            set
            {
                _processedThermal = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProcessedThermal"));
            }
        }

        private double? _ambientTemperature;
        public double? AmbientTemperature
        {
            get => _ambientTemperature;
            set
            {
                _ambientTemperature = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AmbientTemperature"));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ProcessedThermal)));
            }
        }

        private Guid? _frontDropPhotoId;
        public Guid? FrontDropPhotoId
        {
            get => _frontDropPhotoId;
            set
            {
                _frontDropPhotoId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FrontDropPhotoId"));
            }
        }

        private DropPhotoView _frontDropPhoto;
        public DropPhotoView FrontDropPhoto
        {
            get => _frontDropPhoto;
            set
            {
                _frontDropPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FrontDropPhoto"));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Processed)));
            }
        }

        private Guid? _sideDropPhotoId;
        public Guid? SideDropPhotoId
        {
            get => _sideDropPhotoId;
            set
            {
                _sideDropPhotoId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SideDropPhotoId"));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Processed)));
            }
        }

        private DropPhotoView _sideDropPhoto;
        public DropPhotoView SideDropPhoto
        {
            get => _sideDropPhoto;
            set
            {
                _sideDropPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SideDropPhoto"));
            }
        }

        private ThermalPhotoView _thermalPhoto;
        public ThermalPhotoView ThermalPhoto
        {
            get => _thermalPhoto;
            set
            {
                _thermalPhoto = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ThermalPhoto"));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}