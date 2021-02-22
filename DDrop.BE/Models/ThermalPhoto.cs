using System;
using System.Drawing;
using DDrop.BE.Models.Thermal;

namespace DDrop.BE.Models
{
    public class ThermalPhoto : BasePhoto
    {
        public Measurement Measurement { get; set; }

        public FlirImage FlirImage { get; set; }

        public Point EllipseCoordinate { get; set; }

        public Guid? CommentId { get; set; }

        public Comment Comment { get; set; }

        public Guid ContourId { get; set; }

        public Contour Contour { get; set; }
    }
}