using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("BasePhotos")]
    public class DbBasePhoto
    {
        [Key] public Guid PhotoId { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime CreationDateTime { get; set; }

        public string PhotoType { get; set; }
    }
}
