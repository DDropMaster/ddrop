using System;

namespace DDrop.BL.Calculation.DropletSizeCalculator
{
    public static class DropletSizeCalculator
    {
        public static void PerformCalculation(int xDiameterInMeters, int yDiameterInMeters, int zDiameterInMeters, BE.Models.Drop drop)
        {
            if (xDiameterInMeters == 0 && zDiameterInMeters == 0)
            {
                drop.XDiameterInMeters = 0;
                drop.YDiameterInMeters = 0;
                drop.ZDiameterInMeters = 0;
                drop.RadiusInMeters = 0;
                drop.VolumeInCubicalMeters = 0;
            }

            if (xDiameterInMeters != 0 && zDiameterInMeters != 0)
            {
                drop.XDiameterInMeters = xDiameterInMeters / (double)1000;
                drop.YDiameterInMeters = yDiameterInMeters / (double)1000;
                drop.ZDiameterInMeters = zDiameterInMeters / (double)1000;
                drop.VolumeInCubicalMeters = Convert.ToDouble(
                    4f / 3f * Math.PI * (drop.XDiameterInMeters * drop.YDiameterInMeters *
                                         drop.ZDiameterInMeters) / 8);
                drop.RadiusInMeters = Math.Pow(3 * drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);

                return;
            }

            if (xDiameterInMeters != 0)
            {
                drop.XDiameterInMeters = xDiameterInMeters / (double)1000;
                drop.ZDiameterInMeters = 0;
                drop.YDiameterInMeters = yDiameterInMeters / (double)1000;
                drop.VolumeInCubicalMeters = Convert.ToDouble(
                    4f / 3f * Math.PI * (Math.Pow(drop.XDiameterInMeters, 2) * drop.YDiameterInMeters) /
                    8);
                drop.RadiusInMeters = Math.Pow(3 * drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);

                return;
            }

            if (zDiameterInMeters != 0)
            {
                drop.ZDiameterInMeters = zDiameterInMeters / (double)1000;
                drop.XDiameterInMeters = 0;
                drop.YDiameterInMeters = yDiameterInMeters / (double)1000;
                drop.VolumeInCubicalMeters = Convert.ToDouble(
                    4f / 3f * Math.PI * (Math.Pow(drop.ZDiameterInMeters, 2) * drop.YDiameterInMeters) /
                    8);
                drop.RadiusInMeters = Math.Pow(3 * drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);
            }
        }
    }
}