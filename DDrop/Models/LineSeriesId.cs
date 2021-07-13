using System;
using DDrop.Enums;
using LiveCharts.Wpf;

namespace DDrop.Models
{
    public class LineSeriesId : LineSeries
    {
        public Guid Id { get; set; }
        public PlotTypeView plotType { get; set; }
    }
}