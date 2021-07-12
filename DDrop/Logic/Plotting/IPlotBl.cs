using DDrop.Enums;
using DDrop.Models;

namespace DDrop.Logic.Plotting
{
    public interface IPlotBl
    {
        LineSeriesId CreatePlot(PlotView plot, bool combined);
        void AddPoints(PlotView plot, PlotTypeView plotType, SeriesView series, bool dimensionless);
        PlotView CreateBlankPlot(SeriesView series, PlotTypeView plotType, bool dimensionless);
    }
}