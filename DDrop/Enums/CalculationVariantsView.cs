using System.ComponentModel;

namespace DDrop.Enums
{
    public enum CalculationVariantsView
    {
        [Description("Внутренний")] CalculateWithCSharp,
        [Description("Внешний скрипт")] CalculateWithPython,
        [Description("Общий")] Common
    }
}