using System.Text.Json.Serialization;
using DDrop.BE.Enums;

namespace DDrop.BE.Models
{
    public class AutoCalculationSettings
    {
        public bool UseRoi { get; set; }

        public bool UseFrontRoi { get; set; }

        public bool UseSideRoi { get; set; }

        public bool UseThermalRoi { get; set; }

        public ThermalAutoCalculationSettings ThermalAutoCalculationSettings { get; set; }

        public TemperatureDetectingMode TemperatureDetectingMode { get; set; }

        public bool UseFrontRoiIsEnabled { get; set; }

        public bool UseSideRoiIsEnabled { get; set; }

        public bool UseThermalRoiIsEnabled { get; set; }


        [JsonIgnore]
        public Series CurrentSeries { get; set; }
    }
}