using DDrop.Enums;
using DDrop.Models;
using LiveCharts;
using LiveCharts.Definitions.Series;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DDrop.Logic.Plotting
{
    public interface IPlotBl
    {
        LineSeriesId CreatePlot(PlotView plot, bool combined);
        Task<ObservableCollection<SimplePointView>> AddPoints(PlotTypeView plotType, SeriesView series, bool dimensionless);
        PlotView CreateBlankPlot(SeriesView series, PlotTypeView plotType, bool dimensionless);
        ObservableCollection<LineSeriesId> CreateErrorPlots(PlotView plot, bool combined);
    }
}