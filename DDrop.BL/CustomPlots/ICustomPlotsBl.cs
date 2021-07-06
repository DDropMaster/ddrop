using System;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.CustomPlots
{
    public interface ICustomPlotsBl
    {
        Task<Plot> GetPlot(Guid plotId);
        Task<string> GetPlotPoints(Guid plotId);
        Task CreatePlot(Plot plot);
        Task DeletePlot(Plot plot);
        Task UpdatePlot(Plot plot);
        Task UpdatePlotName(string text, Guid plotId);
    }
}