using System;

namespace DDrop.BE.Models
{
    public class DropPhoto : BasePhoto
    {
        public int XDiameterInPixels { get; set; }

        public int YDiameterInPixels { get; set; }

        public int ZDiameterInPixels { get; set; }

        public SimpleLine SimpleHorizontalLine { get; set; }

        public SimpleLine SimpleVerticalLine { get; set; }

        public Guid? ContourId { get; set; }

        public Contour Contour { get; set; }

        public Guid? CommentId { get; set; }

        public Comment Comment { get; set; }

        public Guid MeasurementId { get; set; }
    }
}