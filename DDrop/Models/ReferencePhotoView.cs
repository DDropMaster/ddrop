using System;
using System.Windows.Shapes;

namespace DDrop.Models
{
    public class ReferencePhotoView : BasePhotoView
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

        private SimpleLineView _simpleLine;
        public SimpleLineView SimpleLine
        {
            get => _simpleLine;
            set
            {
                _simpleLine = value;
                RaisePropertyChanged("SimpleLine");
            }
        }

        private Line _line;
        public Line Line
        {
            get => _line;
            set
            {
                _line = value;
                RaisePropertyChanged("Line");
            }
        }

        private int _pixelsInMillimeter;
        public int PixelsInMillimeter
        {
            get => _pixelsInMillimeter;
            set
            {
                _pixelsInMillimeter = value;
                RaisePropertyChanged("PixelsInMillimeter");
            }
        }

        public MeasuringDeviceView _measuringDevice;
        public MeasuringDeviceView MeasuringDevice
        {
            get => _measuringDevice;
            set
            {
                _measuringDevice = value;
                RaisePropertyChanged("MeasuringDevice");
            }
        }

        public bool Editable
        {
            get
            {
                if (PhotoId == Guid.Empty) return false;

                return true;
            }
        }
    }
}