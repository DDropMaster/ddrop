using System;
using System.Windows.Shapes;
using DDrop.Enums;

namespace DDrop.Models
{
    public class DropPhotoView : BasePhotoView
    {
        private int _xDiameterInPixels;
        public int XDiameterInPixels
        {
            get => _xDiameterInPixels;
            set
            {
                _xDiameterInPixels = value;
                RaisePropertyChanged("XDiameterInPixels");
                RaisePropertyChanged(nameof(Processed));
            }
        }

        private int _yDiameterInPixels;
        public int YDiameterInPixels
        {
            get => _yDiameterInPixels;
            set
            {
                _yDiameterInPixels = value;
                RaisePropertyChanged("YDiameterInPixels");
                RaisePropertyChanged(nameof(Processed));
            }
        }

        private int _zDiameterInPixels;
        public int ZDiameterInPixels
        {
            get => _zDiameterInPixels;
            set
            {
                _zDiameterInPixels = value;
                RaisePropertyChanged("ZDiameterInPixels");
                RaisePropertyChanged(nameof(Processed));
            }
        }

        private SimpleLineView _simpleHorizontalLine;
        public SimpleLineView SimpleHorizontalLine
        {
            get => _simpleHorizontalLine;
            set
            {
                _simpleHorizontalLine = value;
                RaisePropertyChanged("SimpleHorizontalLine");
            }
        }

        private Line _horizontalLine;
        public Line HorizontalLine
        {
            get => _horizontalLine;
            set
            {
                _horizontalLine = value;
                RaisePropertyChanged("HorizontalLine");
            }
        }

        private SimpleLineView _simpleVerticalLine;
        public SimpleLineView SimpleVerticalLine
        {
            get => _simpleVerticalLine;
            set
            {
                _simpleVerticalLine = value;
                RaisePropertyChanged("SimpleVerticalLine");
            }
        }

        private Line _verticalLine;
        public Line VerticalLine
        {
            get => _verticalLine;
            set
            {
                _verticalLine = value;
                RaisePropertyChanged("VerticalLine");
            }
        }

        private Guid? _contourId;
        public Guid? ContourId
        {
            get => _contourId;
            set
            {
                _contourId = value;
                RaisePropertyChanged("ContourId");
            }
        }

        private ContourView _contour;
        public ContourView Contour
        {
            get => _contour;
            set
            {
                _contour = value;
                RaisePropertyChanged("Contour");
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

        private bool _processed;
        public bool Processed
        {
            get
            {
                if (PhotoType == PhotoTypeView.FrontDropPhoto && _xDiameterInPixels > 0 && _yDiameterInPixels > 0)
                    return true;

                if (PhotoType == PhotoTypeView.SideDropPhoto && _zDiameterInPixels > 0 && _yDiameterInPixels > 0)
                    return true;

                return false;
            }
            set
            {
                _processed = value;
                RaisePropertyChanged("Processed");
            }
        }

        private bool _editable;
        public bool Editable
        {
            get
            {
                if (PhotoId == Guid.Empty) return false;

                return true;
            }
        }

        private bool _requireSaving;
        public bool RequireSaving
        {
            get => _requireSaving;
            set
            {
                _requireSaving = value;
                RaisePropertyChanged("RequireSaving");
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