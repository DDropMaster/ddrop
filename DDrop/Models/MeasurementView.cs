using System;
using System.ComponentModel;

namespace DDrop.Models
{
    public class MeasurementView : BindableObject
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
                RaisePropertyChanged("CurrentSeriesId");
            }
        }

        public Guid MeasurementId { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private DropView _drop;
        public DropView Drop
        {
            get => _drop;
            set
            {
                _drop = value;
                RaisePropertyChanged("Drop");
                _drop.PropertyChanged += DropPropertyChanged;
            }
        }

        void DropPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Drop");
        }

        public string AddedDate
        {
            get => _addedDate;
            set
            {
                _addedDate = value;
                RaisePropertyChanged("AddedDate");
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

        public string CreationDateTime
        {
            get => _creationDateTime;
            set
            {
                _creationDateTime = value;
                RaisePropertyChanged("CreationDateTime");
            }
        }

        public bool RequireSaving
        {
            get => _requireSaving;
            set
            {
                _requireSaving = value;
                RaisePropertyChanged("RequireSaving");
            }
        }

        public int MeasurementOrderInSeries
        {
            get => _measurementOrderInSeries;
            set
            {
                _measurementOrderInSeries = value;
                RaisePropertyChanged("MeasurementOrderInSeries");
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

                if (ProcessedThermal)
                    return true;

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
                RaisePropertyChanged("ProcessedThermal");
            }
        }

        private double? _ambientTemperature;
        public double? AmbientTemperature
        {
            get => _ambientTemperature;
            set
            {
                _ambientTemperature = value;
                RaisePropertyChanged("AmbientTemperature");
                RaisePropertyChanged(nameof(ProcessedThermal));
            }
        }

        private Guid? _frontDropPhotoId;
        public Guid? FrontDropPhotoId
        {
            get => _frontDropPhotoId;
            set
            {
                _frontDropPhotoId = value;
                RaisePropertyChanged("FrontDropPhotoId");
            }
        }

        private DropPhotoView _frontDropPhoto;
        public DropPhotoView FrontDropPhoto
        {
            get => _frontDropPhoto;
            set
            {
                _frontDropPhoto = value;
                RaisePropertyChanged("FrontDropPhoto");
                RaisePropertyChanged(nameof(Processed));
            }
        }

        private Guid? _sideDropPhotoId;
        public Guid? SideDropPhotoId
        {
            get => _sideDropPhotoId;
            set
            {
                _sideDropPhotoId = value;
                RaisePropertyChanged("SideDropPhotoId");
                RaisePropertyChanged(nameof(Processed));
            }
        }

        private DropPhotoView _sideDropPhoto;
        public DropPhotoView SideDropPhoto
        {
            get => _sideDropPhoto;
            set
            {
                _sideDropPhoto = value;
                RaisePropertyChanged("SideDropPhoto");
            }
        }

        private ThermalPhotoView _thermalPhoto;
        public ThermalPhotoView ThermalPhoto
        {
            get => _thermalPhoto;
            set
            {
                _thermalPhoto = value;
                RaisePropertyChanged("ThermalPhoto");
            }
        }

        private Guid? _commentId;
        public Guid? CommentId
        {
            get => _commentId;
            set
            {
                _commentId = value;
                RaisePropertyChanged("CommentId");
            }
        }

        private CommentView _comment;
        public CommentView Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}