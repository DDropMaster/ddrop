namespace DDrop.Models
{
    public class GeneralSeriesSettingsView : BindableObject
    {
        private bool _useCreationDateTime;
        public bool UseCreationDateTime
        {
            get => _useCreationDateTime;
            set
            {
                _useCreationDateTime = value;
                RaisePropertyChanged("UseCreationDateTime");
            }
        }

        private bool _useThermalPlot;

        public bool UseThermalPlot
        {
            get => _useThermalPlot;
            set
            {
                _useThermalPlot = value;
                RaisePropertyChanged("UseThermalPlot");
            }
        }

        private bool _isAcoustic;
        public bool IsAcoustic
        {
            get => _isAcoustic;
            set
            {
                _isAcoustic = value;
                RaisePropertyChanged("IsAcoustic");
            }
        }
    }
}