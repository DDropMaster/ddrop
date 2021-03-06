using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Measurements")]
    public class DbMeasurement
    {
        [Key] public Guid MeasurementId { get; set; }

        public string Name { get; set; }

        public string AddedDate { get; set; }
        public string CreationDateTime { get; set; }
        public int MeasurementOrderInSeries { get; set; }
        public Guid CurrentSeriesId { get; set; }
        public DbSeries CurrentSeries { get; set; }
 
        public double AmbientTemperature { get; set; }

        public virtual DbDrop Drop { get; set; }

        public Guid? SideDropPhotoId { get; set; }
        [ForeignKey("SideDropPhotoId")]
        public virtual DbDropPhoto SideDropPhoto { get; set; }

        public Guid? FrontDropPhotoId { get; set; }
        [ForeignKey("FrontDropPhotoId")]
        public virtual DbDropPhoto FrontDropPhoto { get; set; }

        public Guid? CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual DbComment Comment { get; set; }

        public virtual DbThermalPhoto ThermalPhoto { get; set; }

        //public double Offset { get; set; }
    }
}