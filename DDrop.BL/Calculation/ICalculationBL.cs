using System.Threading.Tasks;

namespace DDrop.BL.Calculation
{
    public interface ICalculationBL
    {
        Task CalculateDropParameters(BE.Models.Measurement measurement, string pixelsInMillimeter, bool frontProcessed, bool sideProcessed);
        void ReCalculateAllParametersFromLines(BE.Models.Measurement measurement, string pixelsInMillimeterTextBox);
    }
}