using System.ComponentModel;

namespace DDrop.BE.Enums
{
    public enum ThermalAutoCalculationSettings
    {
        [Description("Контуре")] InContour,
        [Description("Области")] InRoi,
    }
}