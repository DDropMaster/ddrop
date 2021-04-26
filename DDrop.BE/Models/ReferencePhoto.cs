using System;

namespace DDrop.BE.Models
{
    public class ReferencePhoto : BasePhoto
    {
        public Guid CurrentSeriesId { get; set; }
        public SimpleLine SimpleLine { get; set; }

        public int PixelsInMillimeter { get; set; }
    }
}