using System;
using System.Drawing;
using System.Windows.Shapes;
using FlirImageView = DDrop.Models.Thermal.FlirImageView;

namespace DDrop.Models
{
    public class ThermalPhotoView : BasePhotoView
    {
        private FlirImageView _flirImage;
        public FlirImageView FlirImage
        {
            get => _flirImage;
            set
            {
                _flirImage = value;
                RaisePropertyChanged("FlirImage");
            }
        }

        private bool _processed;
        public bool Processed
        {
            get
            {
                if (Ellipse != null )
                    return true;

                return false;
            }
            set
            {
                _processed = value;
                RaisePropertyChanged("Processed");
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

        private Ellipse _ellipse;
        public Ellipse Ellipse
        {
            get => _ellipse;
            set
            {
                _ellipse = value;
                RaisePropertyChanged("Ellipse");
            }
        }

        private Point _ellipseCoordinate;
        public Point EllipseCoordinate
        {
            get => _ellipseCoordinate;
            set
            {
                _ellipseCoordinate = value;
                RaisePropertyChanged("EllipseCoordinate");
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
    }
}