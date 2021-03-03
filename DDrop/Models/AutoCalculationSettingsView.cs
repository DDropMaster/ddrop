using System.Text.Json;
using System.Text.Json.Serialization;
using DDrop.Enums;

namespace DDrop.Models
{
    public class AutoCalculationSettingsView : BindableObject
    {
        private bool _useRoi;
        public bool UseRoi
        {
            get => _useRoi;
            set
            {
                _useRoi = value;
                RaisePropertyChanged("UseRoi");
                RaisePropertyChanged(nameof(UseFrontRoi));
                RaisePropertyChanged(nameof(UseSideRoi));
                RaisePropertyChanged(nameof(UseThermalRoi));
                RaisePropertyChanged(nameof(UseThermalRoiIsEnabled));
                RaisePropertyChanged(nameof(UseFrontRoiIsEnabled));
                RaisePropertyChanged(nameof(UseSideRoiIsEnabled));
                RaisePropertyChanged(nameof(ThermalAutoCalculationSettings));
            }
        }

        private bool _useFrontRoi;
        public bool UseFrontRoi
        {
            get
            {
                if (!UseRoi) return false;

                return _useFrontRoi;
            }
            set
            {
                _useFrontRoi = value;
                RaisePropertyChanged("UseFrontRoi");
            }
        }

        private bool _useSideRoi;
        public bool UseSideRoi
        {
            get
            {
                if (!UseRoi) return false;

                return _useSideRoi;
            }
            set
            {
                _useSideRoi = value;
                RaisePropertyChanged("UseSideRoi");
            }
        }

        private bool _useThermalRoi;
        public bool UseThermalRoi
        {
            get
            {
                if (!UseRoi) return false;

                return _useThermalRoi;
            }
            set
            {
                _useThermalRoi = value;
                RaisePropertyChanged("UseThermalRoi");
                RaisePropertyChanged(nameof(ThermalAutoCalculationSettings));
            }
        }

        private ThermalAutoCalculationSettingsView _thermalAutoCalculationSettings;
        public ThermalAutoCalculationSettingsView ThermalAutoCalculationSettings
        {
            get => _thermalAutoCalculationSettings;
            set
            {
                _thermalAutoCalculationSettings = value;

                RaisePropertyChanged("ThermalAutoCalculationSettings");
            }
        }

        private TemperatureDetectingModeView _temperatureDetectingMode;

        public TemperatureDetectingModeView TemperatureDetectingMode
        {
            get => _temperatureDetectingMode;
            set
            {
                _temperatureDetectingMode = value;
                RaisePropertyChanged("TemperatureDetectingMode");
            }
        }

        private bool _useFrontRoiIsEnabled;
        public bool UseFrontRoiIsEnabled
        {
            get
            {
                if (UseRoi)
                {
                    return true;
                }

                return false;
            }
            set
            {
                _useFrontRoiIsEnabled = value;
                RaisePropertyChanged("UseFrontRoiIsEnabled");
            }
        }

        private bool _useSideRoiIsEnabled;
        public bool UseSideRoiIsEnabled
        {
            get
            {
                if (UseRoi)
                {
                    return true;
                }

                return false;
            }
            set
            {
                _useSideRoiIsEnabled = value;
                RaisePropertyChanged("UseSideRoiIsEnabled");
            }
        }

        private bool _useThermalRoiIsEnabled;
        public bool UseThermalRoiIsEnabled
        {
            get
            {
                if (UseRoi)
                {
                    return true;
                }

                return false;
            }
            set
            {
                _useThermalRoiIsEnabled = value;
                RaisePropertyChanged("UseThermalRoiIsEnabled");
            }
        }


        private SeriesView _currentSeries;
        [JsonIgnore]
        public SeriesView CurrentSeries
        {
            get => _currentSeries;
            set
            {
                _currentSeries = value;
                RaisePropertyChanged("CurrentSeries");
            }
        }
    }
}