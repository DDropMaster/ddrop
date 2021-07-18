namespace DDrop.Models
{
    public class ErrorSettingsView : BindableObject
    {
        private double _fullRadiusError;
        public double FullRadiusError
        {
            get => _fullRadiusError;
            set
            {
                _fullRadiusError = value;
                RaisePropertyChanged("FullRadiusError");
            }
        }

        private double _fullTemperatureError;
        public double FullTemperatureError
        {
            get => _fullTemperatureError;
            set
            {
                _fullTemperatureError = value;
                RaisePropertyChanged("FullTemperatureError");
            }
        }
    }
}