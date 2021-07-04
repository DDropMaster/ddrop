using System;
using System.Collections.ObjectModel;
using System.Linq;
using DDrop.Enums;
using DDrop.Models;
using LiveCharts;
using LiveCharts.Defaults;

namespace DDrop.Logic.Plotting
{
    public class PlotBl : IPlotBl
    {
        public LineSeriesId CreatePlot(PlotView plot, bool combined)
        {
            var temp = new ChartValues<ObservablePoint>();

            foreach (var point in plot.Points)
            {
                temp.Add(new ObservablePoint()
                {
                    X = point.X,
                    Y = point.Y
                });
            }

            var scale = 0;

            if (combined && plot.PlotType == PlotTypeView.Temperature)
            {
                scale = 1;
            }

            return new LineSeriesId()
            {
                Id = plot.PlotId,
                Title = plot.Name,
                Values = temp,
                LineSmoothness = 0,
                Fill = System.Windows.Media.Brushes.Transparent,
                ScalesYAt = scale,
                PointGeometry = null
            };
        }

        public PlotView CreatePlot(SeriesView series, PlotTypeView plotType, bool dimensionless)
        {
            var plot = new PlotView()
            {
                CurrentUserId = series.CurrentUserId,
                Name = series.Title,
                PlotId = series.SeriesId,
                IsEditable = false,
                IsDeletable = false,
                Points = new ObservableCollection<SimplePointView>(),
            };

            switch (plotType)
            {
                case PlotTypeView.Radius:
                {
                    for (var j = 0; j < series.MeasurementsSeries.Count; j++)
                    {
                        double time = 0.0;

                        if (series.Settings.GeneralSeriesSettings.UseCreationDateTime)
                        {
                            time = (series.MeasurementsSeries[j].CreationDateTime - series.MeasurementsSeries[0].CreationDateTime).TotalSeconds;
                        }

                        var dropRadiusInMeters = series.MeasurementsSeries[j].Drop.RadiusInMeters;
                        if (dropRadiusInMeters != null)
                        {
                            plot.Points.Add(new SimplePointView()
                            {
                                X = series.Settings.GeneralSeriesSettings.UseCreationDateTime ? time : j * series.IntervalBetweenPhotos,
                                Y = dropRadiusInMeters.Value
                            });
                        }    
                    }

                    if (dimensionless)
                    {
                        var xDiv = plot.Settings?.DimensionlessSettings?.XDimensionlessDivider != null ||
                                    plot.Settings?.DimensionlessSettings?.XDimensionlessDivider != 0
                            ? plot.Settings?.DimensionlessSettings?.XDimensionlessDivider
                            : null;

                        var yDiv = plot.Settings?.DimensionlessSettings?.YDimensionlessDivider != null ||
                                    plot.Settings?.DimensionlessSettings?.YDimensionlessDivider != 0
                            ? plot.Settings?.DimensionlessSettings?.YDimensionlessDivider
                            : null;

                        if (plot.Points.Count > 0)
                        {
                            var initialRadius = plot.Points[0].Y;
                            var wholeEvaporationTime = plot.Points[plot.Points.Count - 1].X;
                            for (int j = 0; j < plot.Points.Count; j++)
                            {
                                plot.Points[j].X = plot.Points[j].X / wholeEvaporationTime;
                                plot.Points[j].Y = plot.Points[j].Y / initialRadius;
                            }
                        }
                    }

                    return plot;
                }
                case PlotTypeView.Temperature:
                {
                    double averageAmbientTemperatures = 0;

                    if (series.Settings.GeneralSeriesSettings.UseThermalPlot)
                    {
                        for (var j = 0; j < series.MeasurementsSeries.Count; j++)
                        {
                            averageAmbientTemperatures += series.MeasurementsSeries[j].AmbientTemperature ?? 0;
                        }

                        plot = series.ThermalPlot;
                        plot.IsEditable = true;
                    }
                    else
                    {
                        for (var j = 0; j < series.MeasurementsSeries.Count; j++)
                        {
                            averageAmbientTemperatures += series.MeasurementsSeries[j].AmbientTemperature ?? 0;
                            var time = (series.MeasurementsSeries[j].CreationDateTime - series.MeasurementsSeries[0].CreationDateTime).TotalSeconds;

                            var dropTemperature = series.MeasurementsSeries[j].Drop.Temperature;
                            if (dropTemperature != null)
                                plot.Points.Add(new SimplePointView()
                                {
                                    X = series.Settings.GeneralSeriesSettings.UseCreationDateTime ? time : j * series.IntervalBetweenPhotos,
                                    Y = dropTemperature.Value
                                });
                        }
                    }

                    if (dimensionless)
                    {
                        if (plot.Points.Count > 0)
                        {
                            averageAmbientTemperatures = averageAmbientTemperatures / series.MeasurementsSeries.Count(x => x.AmbientTemperature != 0);
                            var wholeEvaporationTime = plot.Points[plot.Points.Count - 1].X;
                            for (int j = 0; j < plot.Points.Count; j++)
                            {
                                plot.Points[j].X = plot.Points[j].X / wholeEvaporationTime;
                                plot.Points[j].Y = plot.Points[j].Y / averageAmbientTemperatures;
                            }
                        }
                    }

                    return plot;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(plotType), plotType, null);
            }
        }
    }
}