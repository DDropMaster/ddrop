using System;
using System.Collections.ObjectModel;

namespace DDrop.BE.Models
{
    public class Series
    {
        public Guid CurrentUserId { get; set; }

        public User CurrentUser { get; set; }

        public Guid SeriesId { get; set; }

        public string Title { get; set; }

        public ObservableCollection<Measurement> MeasurementsSeries { get; set; }

        public ReferencePhoto ReferencePhotoForSeries { get; set; }
        
        public SubstanceModel Substance { get; set; }

        public double IntervalBetweenPhotos { get; set; }

        public string AddedDate { get; set; }

        public Guid? CommentId { get; set; }

        public Comment Comment { get; set; }

        public SeriesSettings Settings { get; set; }

        public ObservableCollection<TypedRectangle> RegionOfInterest { get; set; }
    }
}