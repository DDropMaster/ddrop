using System;
using System.Collections.ObjectModel;
using DDrop.BE.Enums;

namespace DDrop.BE.Models
{
    public class Plot
    {
        public Guid PlotId { get; set; }

        public ObservableCollection<SimplePoint> Points { get; set; }

        public string Name { get; set; }

        public Guid CurrentUserId { get; set; }

        public PlotType PlotType { get; set; }

        public PlotSettings Settings { get; set; }
    }
}