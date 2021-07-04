using DDrop.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DDrop.Models
{
    public class MeasurementView : BindableObject
    {
        private Guid _currentSeriesId;
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

        private string _name;
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

        private DateTime _addedDate;
        public DateTime AddedDate
        {
            get => _addedDate;
            set
            {
                _addedDate = value;
                RaisePropertyChanged("AddedDate");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        private DateTime _creationDateTime;
        public DateTime CreationDateTime
        {
            get => _creationDateTime;
            set
            {
                _creationDateTime = value;
                RaisePropertyChanged("CreationDateTime");
            }
        }
        
        private int _measurementOrderInSeries;
        public int MeasurementOrderInSeries
        {
            get => _measurementOrderInSeries;
            set
            {
                _measurementOrderInSeries = value;
                RaisePropertyChanged("MeasurementOrderInSeries");
            }
        }

        public bool Processed
        {
            get
            {
                if (_drop.RadiusInMeters > 0)
                {
                    var frontDropPhoto = _dropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto);

                    var sideDropPhoto = _dropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto);

                    if (frontDropPhoto == null && sideDropPhoto == null)
                        return false;

                    if (frontDropPhoto == null && sideDropPhoto != null && sideDropPhoto.Processed)
                        return true;

                    if (sideDropPhoto == null && frontDropPhoto != null && frontDropPhoto.Processed)
                        return true;

                    if (sideDropPhoto == null && frontDropPhoto != null && !frontDropPhoto.Processed)
                        return false;

                    if (frontDropPhoto == null && sideDropPhoto != null && !sideDropPhoto.Processed)
                        return false;

                    bool twoPhotos = frontDropPhoto.PhotoId != Guid.Empty && sideDropPhoto.PhotoId != Guid.Empty;

                    if (twoPhotos && frontDropPhoto.Processed && sideDropPhoto.Processed)
                        return true;

                    if (twoPhotos && !frontDropPhoto.Processed || !sideDropPhoto.Processed)
                        return false;

                    if (!twoPhotos && frontDropPhoto.Processed || sideDropPhoto.Processed)
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

        private ObservableCollection<DropPhotoView> _dropPhotos;
        public ObservableCollection<DropPhotoView> DropPhotos
        {
            get => _dropPhotos;
            set
            {
                _dropPhotos = value;
                RaisePropertyChanged("DropPhotos");
                RaisePropertyChanged(nameof(Processed));
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
    }
}