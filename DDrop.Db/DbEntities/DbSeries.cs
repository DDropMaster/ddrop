using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Series")]
    public class DbSeries
    {
        [Key] public Guid SeriesId { get; set; }

        public string Title { get; set; }
        public List<DbMeasurement> MeasurementsSeries { get; set; }
        public List<DbReferencePhoto> ReferencePhotoForSeries { get; set; }
        public virtual DbSubstances Substance { get; set; }
        public double IntervalBetweenPhotos { get; set; }
        public string AddedDate { get; set; }
        public Guid CurrentUserId { get; set; }
        public DbUser CurrentUser { get; set; }

        public string RegionOfInterest { get; set; }
        public string Settings { get; set; }

        public Guid? CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual DbComment Comment { get; set; }
        public DbPlot ThermalPlot { get; set; }
    }
}