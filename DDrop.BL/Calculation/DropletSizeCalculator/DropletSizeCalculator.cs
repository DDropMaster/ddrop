using System;

namespace DDrop.BL.Calculation.DropletSizeCalculator
{
    public static class DropletSizeCalculator
    {
        public static void PerformCalculation(int pixelsInMillimeter, int xDiameterInPixels, int yDiameterInPixels, int zDiameterInPixels, BE.Models.Drop drop)
        {
            if (xDiameterInPixels == 0 && zDiameterInPixels == 0)
            {
                drop.XDiameterInMeters = 0;
                drop.YDiameterInMeters = 0;
                drop.ZDiameterInMeters = 0;
                drop.RadiusInMeters = 0;
                drop.VolumeInCubicalMeters = 0;
            }

            if (xDiameterInPixels != 0 && zDiameterInPixels != 0)
            {
                drop.XDiameterInMeters = xDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.YDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.ZDiameterInMeters = zDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.VolumeInCubicalMeters = Convert.ToDouble(
                    4f / 3f * Math.PI * (drop.XDiameterInMeters * drop.YDiameterInMeters *
                                         drop.ZDiameterInMeters) / 8);
                drop.RadiusInMeters = Math.Pow(3 * drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);

                return;
            }

            if (xDiameterInPixels != 0)
            {
                drop.XDiameterInMeters = xDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.ZDiameterInMeters = 0;
                drop.YDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.VolumeInCubicalMeters = Convert.ToDouble(
                    4f / 3f * Math.PI * (Math.Pow(drop.XDiameterInMeters, 2) * drop.YDiameterInMeters) /
                    8);
                drop.RadiusInMeters = Math.Pow(3 * drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);

                return;
            }

            if (zDiameterInPixels != 0)
            {
                drop.ZDiameterInMeters = zDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.XDiameterInMeters = 0;
                drop.YDiameterInMeters = yDiameterInPixels / (double)pixelsInMillimeter / 1000;
                drop.VolumeInCubicalMeters = Convert.ToDouble(
                    4f / 3f * Math.PI * (Math.Pow(drop.ZDiameterInMeters, 2) * drop.YDiameterInMeters) /
                    8);
                drop.RadiusInMeters = Math.Pow(3 * drop.VolumeInCubicalMeters / (4 * Math.PI), 1f / 3f);
            }
        }
    }
}