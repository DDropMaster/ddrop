using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.CustomPlots
{
    public interface ICustomPlotsBl
    {
        Task<Plot> GetPlot(Guid plotId);
        Task<List<SimplePoint>> GetPlotPoints(Guid plotId, double xDimensionlessDivider, double yDimensionlessDivider, bool dimensionless = false);
        Task CreatePlot(Plot plot);
        Task DeletePlot(Plot plot);
        Task UpdatePlot(Plot plot);
        Task UpdatePlotName(string text, Guid plotId);
    }
}