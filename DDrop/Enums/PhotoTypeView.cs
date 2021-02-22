using System.ComponentModel;

namespace DDrop.Enums
{
    public enum PhotoTypeView
    {
        [Description("Фото спереди")] FrontDropPhoto,
        [Description("Фото сбоку")] SideDropPhoto,
        [Description("Термическое")] ThermalPhoto,
        ReferencePhoto,
    }
}