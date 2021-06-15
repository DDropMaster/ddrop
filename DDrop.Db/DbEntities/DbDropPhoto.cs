using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("DropPhotos")]
    public class DbDropPhoto
    {
        [Key] public Guid PhotoId { get; set; }

        public string Name { get; set; }
        public int XDiameterInPixels { get; set; }
        public int YDiameterInPixels { get; set; }
        public int ZDiameterInPixels { get; set; }

        public byte[] Content { get; set; }


        public Guid? ContourId { get; set; }

        [ForeignKey("ContourId")]
        public virtual DbContour Contour { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string PhotoType { get; set; }

        public string HorizontalLine { get; set; }
        public string VerticalLine { get; set; }

        public Guid? CommentId { get; set; }
        [ForeignKey("CommentId")]
        public virtual DbComment Comment { get; set; }
    }
}