using System.ComponentModel;
using System.Windows.Shapes;
using DDrop.Enums;

namespace DDrop.Models
{
    public class TypedRectangleView : BindableObject
    {
        private Rectangle _regionOfInterest;

        public Rectangle RegionOfInterest
        {
            get => _regionOfInterest;
            set
            {
                _regionOfInterest = value;
                RaisePropertyChanged("RegionOfInterest");
            }
        }

        private System.Drawing.Rectangle _rectangle;
        public System.Drawing.Rectangle Rectangle
        {
            get => _rectangle;
            set
            {
                _rectangle = value;
                RaisePropertyChanged("Rectangle");
            }
        }

        private PhotoTypeView _photoType;
        public PhotoTypeView PhotoType
        {
            get => _photoType;
            set
            {
                _photoType = value;
                RaisePropertyChanged("PhotoType");
            }
        }
    }
}