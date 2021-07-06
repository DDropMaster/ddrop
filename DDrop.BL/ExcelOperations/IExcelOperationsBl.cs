using DDrop.BE.Models;
using DDrop.BL.ExcelOperations.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDrop.BL.ExcelOperations
{
    public interface IExcelOperationsBl
    {
        Task CreateSingleSeriesExcelFile(ExcelReport report);
        List<SimplePoint> GetPlotPointsFromFile(string fileName);
    }
}
