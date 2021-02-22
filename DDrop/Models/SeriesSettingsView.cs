using System.ComponentModel;

namespace DDrop.Models
{
    public class SeriesSettingsView : BindableObject
    {
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