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
    }
}