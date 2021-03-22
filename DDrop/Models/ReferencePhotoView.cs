﻿using System.Windows.Shapes;

namespace DDrop.Models
{
    public class ReferencePhotoView : BasePhotoView
    {
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
    }
}