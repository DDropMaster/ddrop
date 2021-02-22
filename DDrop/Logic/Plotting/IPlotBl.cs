using DDrop.Enums;
using DDrop.Models;

namespace DDrop.Logic.Plotting
{
    public interface IPlotBl
    {
        LineSeriesId CreatePlot(PlotView plot, bool combined);
        PlotView CreatePlot(SeriesView series, PlotTypeView plotType, bool dimensionless);
    }
}