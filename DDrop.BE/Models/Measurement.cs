using System;

namespace DDrop.BE.Models
{
    public class Measurement
    {
        public Guid CurrentSeriesId { get; set; }

        public Guid MeasurementId { get; set; }

        public string Name { get; set; }

        public Drop Drop { get; set; }

        public string AddedDate { get; set; }

        public string CreationDateTime { get; set; }

        public int MeasurementOrderInSeries { get; set; }

        public double? AmbientTemperature { get; set; }

        public Guid? FrontDropPhotoId { get; set; }

        public DropPhoto FrontDropPhoto { get; set; }

        public Guid? SideDropPhotoId { get; set; }

        public DropPhoto SideDropPhoto { get; set; }

        public ThermalPhoto ThermalPhoto { get; set; }

        public Guid? CommentId { get; set; }

        public Comment Comment { get; set; }

        public bool Processed { get; set; }
    }
}