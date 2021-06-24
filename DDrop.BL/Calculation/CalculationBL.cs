using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DDrop.BE.Enums;
using DDrop.BE.Models;
using DDrop.BL.Drop;
using DDrop.Utility.Calculation;

namespace DDrop.BL.Calculation
{
    public class CalculationBL : ICalculationBL
    {
        private readonly IDropBL _dropBl;

        public CalculationBL(IDropBL dropBl)
        {
            _dropBl = dropBl;
        }

        public async Task<BE.Models.Drop> CalculateDropParameters(BE.Models.Measurement measurement, List<ReferencePhoto> referencePhotos, bool frontProcessed, bool sideProcessed)
        {
            var frontReference = referencePhotos
                .FirstOrDefault(x => x?.PhotoType == PhotoType.FrontDropPhoto);

            var sideReference = referencePhotos
                .FirstOrDefault(x => x?.PhotoType == PhotoType.SideDropPhoto);

            var frontDropPhoto = measurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoType.FrontDropPhoto);

            var sideDropPhoto = measurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoType.SideDropPhoto);

            if (frontDropPhoto != null && frontProcessed && sideDropPhoto != null && sideProcessed)
            {
                if (frontReference?.PixelsInMillimeter == 0 || sideReference?.PixelsInMillimeter == 0)
                {
                    throw new InvalidOperationException("Укажите референсное расстояние");
                }

                var yDiameterInMillimeters = (frontDropPhoto.YDiameterInPixels / (double)frontReference.PixelsInMillimeter +
                                         sideDropPhoto.YDiameterInPixels / (double)sideReference.PixelsInMillimeter) / 2;

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    frontDropPhoto.XDiameterInPixels / (double)frontReference.PixelsInMillimeter,
                    yDiameterInMillimeters,
                    sideDropPhoto.ZDiameterInPixels / (double)sideReference.PixelsInMillimeter, 
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
            else if ((frontDropPhoto == null || !frontProcessed) && sideDropPhoto != null && sideProcessed)
            {
                if (sideReference?.PixelsInMillimeter == 0)
                {
                    throw new InvalidOperationException("Укажите референсное расстояние");
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    0,
                    sideDropPhoto.YDiameterInPixels / (double)sideReference.PixelsInMillimeter,
                    sideDropPhoto.ZDiameterInPixels / (double)sideReference.PixelsInMillimeter,
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
            else if (frontDropPhoto != null && frontProcessed && (sideDropPhoto == null || !sideProcessed))
            {
                if (frontReference?.PixelsInMillimeter == 0)
                {
                    throw new InvalidOperationException("Укажите референсное расстояние");
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    frontDropPhoto.XDiameterInPixels / (double)frontReference.PixelsInMillimeter,
                    frontDropPhoto.YDiameterInPixels / (double)frontReference.PixelsInMillimeter,
                    0,
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }

            return measurement.Drop;
        }

        public BE.Models.Measurement ReCalculateAllParametersFromLines(BE.Models.Measurement measurement, List<ReferencePhoto> referencePhotos)
        {
            var frontReferencePixelsInMillimeter = referencePhotos
                .FirstOrDefault(x => x?.PhotoType == PhotoType.FrontDropPhoto)?.PixelsInMillimeter;

            var sideReferencePixelsInMillimeter = referencePhotos
                .FirstOrDefault(x => x?.PhotoType == PhotoType.SideDropPhoto)?.PixelsInMillimeter;

            var frontDropPhoto = measurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoType.FrontDropPhoto);

            var sideDropPhoto = measurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoType.SideDropPhoto);

            if (frontDropPhoto.PhotoId != null && sideDropPhoto.PhotoId != null)
            {
                if (frontDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.Y2));
                    frontDropPhoto.XDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.XDiameterInPixels = 0;
                }

                if (frontDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.Y2));
                    frontDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.YDiameterInPixels = 0;
                }


                if (sideDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.Y2));
                    sideDropPhoto.ZDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.ZDiameterInPixels = 0;
                }

                if (sideDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.Y2));
                    sideDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.YDiameterInPixels = 0;
                }

                var yDiameterInMillimeters = (frontDropPhoto.YDiameterInPixels / frontReferencePixelsInMillimeter +
                                              sideDropPhoto.YDiameterInPixels / sideReferencePixelsInMillimeter) / 2;

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    frontDropPhoto.XDiameterInPixels,
                    yDiameterInMillimeters.Value,
                    sideDropPhoto.ZDiameterInPixels,
                    measurement.Drop);
            }
            else if (frontDropPhoto.PhotoId == null && sideDropPhoto.PhotoId != null)
            {
                if (sideDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(sideDropPhoto.SimpleHorizontalLine.Y2));
                    sideDropPhoto.ZDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.ZDiameterInPixels = 0;
                }

                if (sideDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(sideDropPhoto.SimpleVerticalLine.Y2));
                    sideDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.YDiameterInPixels = 0;
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    0,
                    sideDropPhoto.YDiameterInPixels / sideReferencePixelsInMillimeter.Value,
                    sideDropPhoto.ZDiameterInPixels / sideReferencePixelsInMillimeter.Value,
                    measurement.Drop);
            }
            else if (frontDropPhoto.PhotoId != null && sideDropPhoto.PhotoId == null)
            {
                if (frontDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(frontDropPhoto.SimpleHorizontalLine.Y2));
                    frontDropPhoto.XDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.XDiameterInPixels = 0;
                }

                if (frontDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(frontDropPhoto.SimpleVerticalLine.Y2));
                    frontDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.YDiameterInPixels = 0;
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    frontDropPhoto.XDiameterInPixels / frontReferencePixelsInMillimeter.Value,
                    frontDropPhoto.YDiameterInPixels / frontReferencePixelsInMillimeter.Value,
                    0,
                    measurement.Drop);
            }

            return measurement;
        }
    }
}