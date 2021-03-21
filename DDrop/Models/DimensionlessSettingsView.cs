namespace DDrop.Models
{
    public class DimensionlessSettingsView : BindableObject
    {
        private double _xDimensionlessDivider;
        public double XDimensionlessDivider
        {
            get => _xDimensionlessDivider;
            set
            {
                _xDimensionlessDivider = value;
                RaisePropertyChanged("XDimensionlessDivider");
            }
        }

        private double _yDimensionlessDivider;
        public double YDimensionlessDivider
        {
            get => _yDimensionlessDivider;
            set
            {
                _yDimensionlessDivider = value;
                RaisePropertyChanged("YDimensionlessDivider");
            }
        }
    }
}