using System;
using System.Collections.ObjectModel;
using DDrop.Enums;

namespace DDrop.Models
{
    public class PlotView : BindableObject
    {
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

        private Guid _plotId;
        public Guid PlotId
        {
            get => _plotId;
            set
            {
                _plotId = value;
                RaisePropertyChanged("PlotId");
            }
        }

        private ObservableCollection<SimplePointView> _points;
        public ObservableCollection<SimplePointView> Points
        {
            get => _points;
            set
            {
                _points = value;
                RaisePropertyChanged("Points");
            }
        }

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

        private Guid _seriesId;
        public Guid SeriesId
        {
            get => _seriesId;
            set
            {
                _seriesId = value;
                RaisePropertyChanged("SeriesId");
            }
        }

        private PlotTypeView _plotType;

        public PlotTypeView PlotType
        {
            get => _plotType;
            set
            {
                _plotType = value;
                RaisePropertyChanged("PlotType");
            }
        }

        private PlotSettingsView _settings;
        public PlotSettingsView Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                RaisePropertyChanged("Settings");
            }
        }

        private bool _isEditable;
        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                _isEditable = value;
                RaisePropertyChanged("IsEditable");
            }
        }

        private bool _isDeletable;

        public bool IsDeletable
        {
            get => _isDeletable;
            set
            {
                _isDeletable = value;
                RaisePropertyChanged("IsDeletable");
            }
        }
    }
}