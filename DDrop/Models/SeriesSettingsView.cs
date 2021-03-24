namespace DDrop.Models
{
    public class SeriesSettingsView : BindableObject
    {
        private GeneralSeriesSettingsView _generalSeriesSettings;
        public GeneralSeriesSettingsView GeneralSeriesSettings
        {
            get => _generalSeriesSettings;
            set
            {
                _generalSeriesSettings = value;
                RaisePropertyChanged("GeneralSeriesSettings");
            }
        }

        private AutoCalculationSettingsView _autoCalculationSettings;
        public AutoCalculationSettingsView AutoCalculationSettings
        {
            get => _autoCalculationSettings;
            set
            {
                _autoCalculationSettings = value;
                RaisePropertyChanged("AutoCalculationSettings");
            }
        }
    }
}