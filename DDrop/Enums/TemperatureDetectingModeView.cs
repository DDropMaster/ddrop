using System.ComponentModel;

namespace DDrop.Enums
{
    public enum TemperatureDetectingModeView
    {
        [Description("Максимальную температуру")] Max,
        [Description("Минимальную температуру")] Min,
        [Description("Среднюю температуру")] Average
    }
}