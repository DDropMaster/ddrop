using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("ReferencePhotos")]
    public class DbReferencePhoto : DbBasePhoto
    {
        public int PixelsInMillimeter { get; set; }
        public string ReferenceLine { get; set; }
        public Guid CurrentSeriesId { get; set; }
        public string MeasuringDevice { get; set; }
        public DbSeries Series { get; set; }
    }
}