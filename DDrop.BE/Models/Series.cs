using System;
using System.Collections.Generic;

namespace DDrop.BE.Models
{
    public class Series
    {
        public Guid CurrentUserId { get; set; }

        public Guid SeriesId { get; set; }

        public string Title { get; set; }

        public List<Measurement> MeasurementsSeries { get; set; }

        public List<ReferencePhoto> ReferencePhotoForSeries { get; set; }
        
        public SubstanceModel Substance { get; set; }

        public double IntervalBetweenPhotos { get; set; }

        public string AddedDate { get; set; }

        public Guid? CommentId { get; set; }

        public Comment Comment { get; set; }

        public SeriesSettings Settings { get; set; }

        public List<TypedRectangle> RegionOfInterest { get; set; }
        public Plot ThermalPlot { get; set; }

        public bool Processed { get; set; }
    }
}