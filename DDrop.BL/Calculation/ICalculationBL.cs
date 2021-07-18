using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.Calculation
{
    public interface ICalculationBL
    {
        Task<BE.Models.Drop> CalculateDropParameters(BE.Models.Measurement measurement, List<ReferencePhoto> referencePhotos, bool frontProcessed, bool sideProcessed);
        BE.Models.Measurement ReCalculateAllParametersFromLines(BE.Models.Measurement measurement, List<ReferencePhoto> referencePhotos);
        //double CalculateFullRadiusError(List<double> referenceValues, )
    }
}