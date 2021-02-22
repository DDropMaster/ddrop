using System.ComponentModel;
using System.Windows.Shapes;

namespace DDrop.Models
{
    public class ReferencePhotoView : BasePhotoView
    {
        private Line _line;

        private int _pixelsInMillimeter;
        private SeriesView _series;
        private SimpleLineView _simpleLine;

        public SeriesView Series
        {
            get => _series;
            set
            {
                _series = value;
                RaisePropertyChanged("Series");
            }
        }

        public SimpleLineView SimpleLine
        {
            get => _simpleLine;
            set
            {
                _simpleLine = value;
                RaisePropertyChanged("SimpleLine");
            }
        }

        public Line Line
        {
            get => _line;
            set
            {
                _line = value;
                RaisePropertyChanged("Line");
            }
        }

        public int PixelsInMillimeter
        {
            get => _pixelsInMillimeter;
            set
            {
                _pixelsInMillimeter = value;
                RaisePropertyChanged("PixelsInMillimeter");
            }
        }
    }
}