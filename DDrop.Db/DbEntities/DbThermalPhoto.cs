using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("ThermalPhotos")]
    public class DbThermalPhoto
    {
        [Key] public Guid PhotoId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string AddedDate { get; set; }
        public string CreationDateTime { get; set; }
        public string PhotoType { get; set; }
        public virtual DbMeasurement Measurement { get; set; }
        public string EllipseCoordinate { get; set; }

        public Guid? ContourId { get; set; }

        [ForeignKey("ContourId")]
        public virtual DbContour Contour { get; set; }

        public Guid? CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual DbComment Comment { get; set; }
    }
}