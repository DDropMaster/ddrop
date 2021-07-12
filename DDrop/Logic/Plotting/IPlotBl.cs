using DDrop.Enums;
using DDrop.Models;
using System.Collections.ObjectModel;

namespace DDrop.Logic.Plotting
{
    public interface IPlotBl
    {
        LineSeriesId CreatePlot(PlotView plot, bool combined);
        ObservableCollection<SimplePointView> AddPoints(PlotTypeView plotType, SeriesView series, bool dimensionless);
        PlotView CreateBlankPlot(SeriesView series, PlotTypeView plotType, bool dimensionless);
    }
}