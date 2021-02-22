using System;

namespace DDrop.BE.Models
{
    public class Drop
    {
        public Series Series { get; set; }

        public Guid DropId { get; set; }

        public double XDiameterInMeters { get; set; }

        public double YDiameterInMeters { get; set; }

        public double ZDiameterInMeters { get; set; }

        public double VolumeInCubicalMeters { get; set; }

        public double? RadiusInMeters { get; set; }

        public double? Temperature { get; set; }

        public Measurement Measurement { get; set; }
    }
}