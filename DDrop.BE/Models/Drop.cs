using System;

namespace DDrop.BE.Models
{
    public class Drop
    {
        public Guid DropId { get; set; }

        public double XDiameterInMeters { get; set; }

        public double YDiameterInMeters { get; set; }

        public double ZDiameterInMeters { get; set; }

        public double VolumeInCubicalMeters { get; set; }

        public double? RadiusInMeters { get; set; }

        public double? Temperature { get; set; }
    }
}