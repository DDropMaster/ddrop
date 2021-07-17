using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace DDrop.Models
{
    public class SeriesView : BindableObject
    {
        public SeriesView()
        {
            _measurementsSeries = new ObservableCollection<MeasurementView>();
            _measurementsSeries.CollectionChanged += _measurementsSeries_CollectionChanged;
        }

        private Guid _currentUserId;
        public Guid CurrentUserId
        {
            get => _currentUserId;
            set
            {
                _currentUserId = value;
                RaisePropertyChanged("CurrentUserId");
            }
        }

        public Guid SeriesId { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        private ObservableCollection<MeasurementView> _measurementsSeries;
        public ObservableCollection<MeasurementView> MeasurementsSeries
        {
            get => _measurementsSeries;
            set
            {
                _measurementsSeries = value;
                RaisePropertyChanged("MeasurementsSeries");
                RaisePropertyChanged("CanDrawPlot");

                foreach (var measurementsSeries in _measurementsSeries)
                {
                    measurementsSeries.PropertyChanged += MeasurementPropertyChanged;
                }
            }
        }

        void MeasurementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("MeasurementsSeries");
        }

        private ObservableCollection<ReferencePhotoView> _referencePhotoForSeries;
        public ObservableCollection<ReferencePhotoView> ReferencePhotoForSeries
        {
            get => _referencePhotoForSeries;
            set
            {
                _referencePhotoForSeries = value;
                RaisePropertyChanged("ReferencePhotoForSeries");
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

                    RaisePropertyChanged(nameof(Substance));
                }

                RaisePropertyChanged("Sub");
            }
        }

        private SubstanceModelView _substance;
        public SubstanceModelView Substance
        {
            get => _substance;
            set
            {
                _substance = value;
                RaisePropertyChanged("Substance");
                RaisePropertyChanged(nameof(IsSubstanceExists));
            }
        }

        private bool _exactCalculationModel;
        public bool ExactCalculationModel
        {
            get => _exactCalculationModel;
            set
            {
                _exactCalculationModel = value;
                RaisePropertyChanged("ExactCalculationModel");
            }
        }

        private double _intervalBetweenPhotos;
        public double IntervalBetweenPhotos
        {
            get => _intervalBetweenPhotos;
            set
            {
                _intervalBetweenPhotos = value;
                RaisePropertyChanged("IntervalBetweenPhotos");
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

        private bool _canDrawPlot;
        public bool CanDrawPlot
        {
            get
            {
                if (_measurementsSeries?.Count > 0 && _measurementsSeries.All(x => x.Processed))
                    return true;

                return false;
            }
            set
            {
                _canDrawPlot = value;
                RaisePropertyChanged("CanDrawPlot");
                RaisePropertyChanged(nameof(CanDrawAnyPlot));
            }
        }

        private bool _canDrawTemperaturePlot;
        public bool CanDrawTemperaturePlot
        {
            get
            {
                if (ThermalPlot != null && Settings.GeneralSeriesSettings.UseThermalPlot)
                {
                    return true;
                }

                if (_measurementsSeries.Count > 0 && _measurementsSeries.Any(x => x.ThermalPhoto != null) && _measurementsSeries.Where(x => x.ThermalPhoto != null).All(x => x.ProcessedThermal))
                {
                    return true;
                }

                return false;
            }
            set
            {
                _canDrawTemperaturePlot = value;
                RaisePropertyChanged("CanDrawTemperaturePlot");
                RaisePropertyChanged(nameof(CanDrawAnyPlot));
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
                RaisePropertyChanged("CanDrawAnyPlot");
            }
        }

        private bool _isSubstanceEdited;
        public bool IsSubstanceEdited
        {
            get => _isSubstanceEdited;
            set
            {
                _isSubstanceEdited = value;
                RaisePropertyChanged("IsSubstanceEdited");
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
                RaisePropertyChanged("IsSubstanceExists");
            }
        }

        private bool _loaded = false;
        public bool Loaded
        {
            get => _loaded;
            set
            {
                _loaded = value;
                RaisePropertyChanged("Loaded");
            }
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

        private SeriesSettingsView _settings;
        public SeriesSettingsView Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                RaisePropertyChanged("MeasuringDevice");
            }
        }

        private ObservableCollection<TypedRectangleView> _regionOfInterest;
        public ObservableCollection<TypedRectangleView> RegionOfInterest
        {
            get => _regionOfInterest;
            set
            {
                _regionOfInterest = value;
                RaisePropertyChanged("RegionOfInterest");
            }
        }

        private PlotView _thermalPlot;

        public PlotView ThermalPlot
        {
            get => _thermalPlot;
            set
            {
                _thermalPlot = value;
                RaisePropertyChanged("ThermalPlot");
            }
        }

        private void _measurementsSeries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CanDrawPlot));
            foreach (var photo in _measurementsSeries) photo.MeasurementOrderInSeries = _measurementsSeries.IndexOf(photo);
        }
    }
}