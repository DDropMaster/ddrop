namespace DDrop.Models
{
    public class PlotSettingsView : BindableObject
    {
        private DimensionlessSettingsView _dimensionlessSettings;
        public DimensionlessSettingsView DimensionlessSettings
        {
            get => _dimensionlessSettings;
            set
            {
                _dimensionlessSettings = value;
                RaisePropertyChanged("DimensionlessSettings");
            }
        }

        private double _error;
        public double Error
        {
            get => _error;
            set
            {
                _error = value;
                RaisePropertyChanged("Error");
            }
        }
    }
}