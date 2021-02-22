using DDrop.BE.Models.Thermal;

namespace DDrop.BL.Radiometric.ThermalDataExtractor
{
    public interface IThermalBL
    {
        FlirImage ProcessImage(string imageFilePath);
    }
}