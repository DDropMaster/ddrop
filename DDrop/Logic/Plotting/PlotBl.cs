﻿using System;
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

        public PlotView CreateBlankPlot(SeriesView series, PlotTypeView plotType, bool dimensionless)
        {
            return new PlotView()
            {
                CurrentUserId = series.CurrentUserId,
                Name = series.Title,
                PlotId = series.SeriesId,
                IsEditable = false,
                IsDeletable = false,
                Points = new ObservableCollection<SimplePointView>(),
                SeriesId = series.SeriesId
            };
        }

        public ObservableCollection<SimplePointView> AddPoints(PlotTypeView plotType, SeriesView series, bool dimensionless)
        {
            switch (plotType)
            {
                case PlotTypeView.Radius:
                {
                    var points = new ObservableCollection<SimplePointView>();

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
                            points.Add(new SimplePointView()
                            {
                                X = series.Settings.GeneralSeriesSettings.UseCreationDateTime ? time : j * series.IntervalBetweenPhotos,
                                Y = dropRadiusInMeters.Value
                            });
                        }
                    }

                    if (dimensionless)
                    {
                        if (points.Count > 0)
                        {
                            var initialRadius = points[0].Y;
                            var wholeEvaporationTime = points[points.Count - 1].X;
                            for (int j = 0; j < points.Count; j++)
                            {
                                points[j].X = points[j].X / wholeEvaporationTime;
                                points[j].Y = points[j].Y / initialRadius;
                            }
                        }
                    }
                    return points;
                }
                case PlotTypeView.Temperature:
                {
                    var points = new ObservableCollection<SimplePointView>();
                    double averageAmbientTemperatures = 0;

                    if (series.Settings.GeneralSeriesSettings.UseThermalPlot)
                    {
                        for (var j = 0; j < series.MeasurementsSeries.Count; j++)
                        {
                            averageAmbientTemperatures += series.MeasurementsSeries[j].AmbientTemperature ?? 0;
                        }

                        points = series.ThermalPlot.Points;
                    }
                    else
                    {
                        for (var j = 0; j < series.MeasurementsSeries.Count; j++)
                        {
                            averageAmbientTemperatures += series.MeasurementsSeries[j].AmbientTemperature ?? 0;
                            var time = (series.MeasurementsSeries[j].CreationDateTime - series.MeasurementsSeries[0].CreationDateTime).TotalSeconds;

                            var dropTemperature = series.MeasurementsSeries[j].Drop.Temperature;
                            if (dropTemperature != null)
                                points.Add(new SimplePointView()
                                {
                                    X = series.Settings.GeneralSeriesSettings.UseCreationDateTime ? time : j * series.IntervalBetweenPhotos,
                                    Y = dropTemperature.Value
                                });
                        }
                    }

                    if (dimensionless)
                    {
                        if (points.Count > 0)
                        {
                            averageAmbientTemperatures = averageAmbientTemperatures / series.MeasurementsSeries.Count(x => x.AmbientTemperature != 0);
                            var wholeEvaporationTime = points[points.Count - 1].X;
                            for (int j = 0; j < points.Count; j++)
                            {
                                points[j].X = points[j].X / wholeEvaporationTime;
                                points[j].Y = points[j].Y / averageAmbientTemperatures;
                            }
                        }
                    }

                    return points;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(plotType), plotType, null);
            }
        }
    }
}