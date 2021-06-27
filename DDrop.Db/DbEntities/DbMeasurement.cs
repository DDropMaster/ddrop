using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Measurements")]
    public class DbMeasurement
    {
        [Key] public Guid MeasurementId { get; set; }

        public string Name { get; set; }

        public DateTime AddedDate { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int MeasurementOrderInSeries { get; set; }
        public Guid CurrentSeriesId { get; set; }
        public DbSeries CurrentSeries { get; set; }
 
        public double AmbientTemperature { get; set; }

        public virtual DbDrop Drop { get; set; }

        public Guid? CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual DbComment Comment { get; set; }

        public virtual DbThermalPhoto ThermalPhoto { get; set; }

        public List<DbDropPhoto> DropPhotos { get; set; }
    }
}