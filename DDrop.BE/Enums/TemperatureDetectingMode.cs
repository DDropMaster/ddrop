using System.ComponentModel;

namespace DDrop.BE.Enums
{
    public enum TemperatureDetectingMode
    {
        [Description("Максимальную температуру")] Max,
        [Description("Минимальную температуру")] Min,
        [Description("Среднюю температуру")] Average
    }
}