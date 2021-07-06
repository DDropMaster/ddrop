using System;
using System.Collections.Generic;
using DDrop.BE.Enums;

namespace DDrop.BE.Models
{
    public class Plot
    {
        public Guid PlotId { get; set; }

        public List<SimplePoint> Points { get; set; }

        public string Name { get; set; }

        public Guid CurrentUserId { get; set; }

        public Guid SeriesId { get; set; }

        public PlotType PlotType { get; set; }

        public PlotSettings Settings { get; set; }
    }
}