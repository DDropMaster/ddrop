using System;

namespace DDrop.Models
{
    public class DropView : BindableObject
    {
        private double? _radiusInMeters;
        private double _volumeInCubicalMeters;
        private double _xDiameterInMeters;

        private double _yDiameterInMeters;

        private double _zDiameterInMeters;

        private double? _temperature;

        public Guid DropId { get; set; }

        public double XDiameterInMeters
        {
            get => _xDiameterInMeters;
            set
            {
                _xDiameterInMeters = value;
                RaisePropertyChanged("XDiameterInMeters");
            }
        }

        public double YDiameterInMeters
        {
            get => _yDiameterInMeters;
            set
            {
                _yDiameterInMeters = value;
                RaisePropertyChanged("YDiameterInMeters");
            }
        }

        public double ZDiameterInMeters
        {
            get => _zDiameterInMeters;
            set
            {
                _zDiameterInMeters = value;
                RaisePropertyChanged("ZDiameterInMeters");
            }
        }

        public double VolumeInCubicalMeters
        {
            get => _volumeInCubicalMeters;
            set
            {
                _volumeInCubicalMeters = value;
                RaisePropertyChanged("VolumeInCubicalMeters");
            }
        }

        public double? RadiusInMeters
        {
            get => _radiusInMeters;
            set
            {
                _radiusInMeters = value;
                RaisePropertyChanged("RadiusInMeters");
            }
        }

        public double? Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }
    }
}