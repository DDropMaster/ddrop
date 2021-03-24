using System.ComponentModel;

namespace DDrop.BE.Enums
{
    public enum PhotoType
    {
        [Description("Фото спереди")] FrontDropPhoto,
        [Description("Фото сбоку")] SideDropPhoto,
        [Description("Термическое")] ThermalPhoto,
    }
}