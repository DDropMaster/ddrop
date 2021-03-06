using System.Threading.Tasks;

namespace DDrop.BL.Calculation
{
    public interface ICalculationBL
    {
        Task<BE.Models.Drop> CalculateDropParameters(BE.Models.Measurement measurement, string pixelsInMillimeter, bool frontProcessed, bool sideProcessed);
        BE.Models.Measurement ReCalculateAllParametersFromLines(BE.Models.Measurement measurement, string pixelsInMillimeterTextBox);
    }
}