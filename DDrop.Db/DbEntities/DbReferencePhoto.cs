using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("ReferencePhotos")]
    public class DbReferencePhoto
    {
        [Key] public Guid PhotoId { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public int PixelsInMillimeter { get; set; }
        public string ReferenceLine { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string PhotoType { get; set; }
        public Guid CurrentSeriesId { get; set; }
        public DbSeries Series { get; set; }
    }
}