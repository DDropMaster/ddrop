using System;
using System.ComponentModel;

namespace DDrop.Models
{
    public class DropView : INotifyPropertyChanged
    {
        private double? _radiusInMeters;
        private SeriesView _series;

        private double _volumeInCubicalMeters;
        private double _xDiameterInMeters;

        private double _yDiameterInMeters;

        private double _zDiameterInMeters;

        private double? _temperature;

        public SeriesView Series
        {
            get => _series;
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Series"));
            }
        }

        public Guid DropId { get; set; }

        public double XDiameterInMeters
        {
            get => _xDiameterInMeters;
            set
            {
                _xDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XDiameterInMeters"));
            }
        }

        public double YDiameterInMeters
        {
            get => _yDiameterInMeters;
            set
            {
                _yDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YDiameterInMeters"));
            }
        }

        public double ZDiameterInMeters
        {
            get => _zDiameterInMeters;
            set
            {
                _zDiameterInMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZDiameterInMeters"));
            }
        }

        public double VolumeInCubicalMeters
        {
            get => _volumeInCubicalMeters;
            set
            {
                _volumeInCubicalMeters = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VolumeInCubicalMeters"));
            }
        }

        public double? RadiusInMeters
        {
            get => _radiusInMeters;
            set
            {
                _radiusInMeters = value;
                _series?.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Series.CanDrawPlot)));
                _measurement?.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Measurement.Processed)));
                OnPropertyChanged(new PropertyChangedEventArgs("RadiusInMeters"));
            }
        }

        public double? Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Temperature"));
            }
        }


        private MeasurementView _measurement;

        public MeasurementView Measurement
        {
            get => _measurement;
            set
            {
                _measurement = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Measurement"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}