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
                .FirstOrDefault(x => x.PhotoType == PhotoType.FrontDropPhoto);

            var sideReference = referencePhotos
                .FirstOrDefault(x => x.PhotoType == PhotoType.SideDropPhoto);

            if (measurement.FrontDropPhoto != null && frontProcessed && measurement.SideDropPhoto != null && sideProcessed)
            {
                if (frontReference?.PixelsInMillimeter == 0 || sideReference?.PixelsInMillimeter == 0)
                {
                    throw new InvalidOperationException("Укажите референсное расстояние");
                }

                var yDiameterInMillimeters = (measurement.FrontDropPhoto.YDiameterInPixels / (double)frontReference.PixelsInMillimeter +
                                         measurement.SideDropPhoto.YDiameterInPixels / (double)sideReference.PixelsInMillimeter) / 2;

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    measurement.FrontDropPhoto.XDiameterInPixels / (double)frontReference.PixelsInMillimeter,
                    yDiameterInMillimeters,
                    measurement.SideDropPhoto.ZDiameterInPixels / (double)sideReference.PixelsInMillimeter, 
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
            else if ((measurement.FrontDropPhoto == null || !frontProcessed) && measurement.SideDropPhoto != null && sideProcessed)
            {
                if (sideReference?.PixelsInMillimeter == 0)
                {
                    throw new InvalidOperationException("Укажите референсное расстояние");
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    0,
                    measurement.SideDropPhoto.YDiameterInPixels / (double)sideReference.PixelsInMillimeter,
                    measurement.SideDropPhoto.ZDiameterInPixels / (double)sideReference.PixelsInMillimeter,
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
            else if (measurement.FrontDropPhoto != null && frontProcessed && (measurement.SideDropPhoto == null || !sideProcessed))
            {
                if (frontReference?.PixelsInMillimeter == 0)
                {
                    throw new InvalidOperationException("Укажите референсное расстояние");
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    measurement.FrontDropPhoto.XDiameterInPixels / (double)frontReference.PixelsInMillimeter,
                    measurement.FrontDropPhoto.YDiameterInPixels / (double)frontReference.PixelsInMillimeter,
                    0,
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }

            return measurement.Drop;
        }

        public BE.Models.Measurement ReCalculateAllParametersFromLines(BE.Models.Measurement measurement, List<ReferencePhoto> referencePhotos)
        {
            var frontReferencePixelsInMillimeter = referencePhotos
                .FirstOrDefault(x => x.PhotoType == PhotoType.FrontDropPhoto).PixelsInMillimeter;

            var sideReferencePixelsInMillimeter = referencePhotos
                .FirstOrDefault(x => x.PhotoType == PhotoType.SideDropPhoto).PixelsInMillimeter;

            if (measurement.FrontDropPhotoId != null && measurement.SideDropPhotoId != null)
            {
                if (measurement.FrontDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.Y2));
                    measurement.FrontDropPhoto.XDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    measurement.FrontDropPhoto.XDiameterInPixels = 0;
                }

                if (measurement.FrontDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.Y2));
                    measurement.FrontDropPhoto.YDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    measurement.FrontDropPhoto.YDiameterInPixels = 0;
                }


                if (measurement.SideDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.Y2));
                    measurement.SideDropPhoto.ZDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    measurement.SideDropPhoto.ZDiameterInPixels = 0;
                }

                if (measurement.SideDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.Y2));
                    measurement.SideDropPhoto.YDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    measurement.SideDropPhoto.YDiameterInPixels = 0;
                }

                var yDiameterInMillimeters = (measurement.FrontDropPhoto.YDiameterInPixels / frontReferencePixelsInMillimeter +
                                              measurement.SideDropPhoto.YDiameterInPixels / sideReferencePixelsInMillimeter) / 2;

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    measurement.FrontDropPhoto.XDiameterInPixels,
                    yDiameterInMillimeters,
                    measurement.SideDropPhoto.ZDiameterInPixels,
                    measurement.Drop);
            }
            else if (measurement.FrontDropPhotoId == null && measurement.SideDropPhotoId != null)
            {
                if (measurement.SideDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleHorizontalLine.Y2));
                    measurement.SideDropPhoto.ZDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    measurement.SideDropPhoto.ZDiameterInPixels = 0;
                }

                if (measurement.SideDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(measurement.SideDropPhoto.SimpleVerticalLine.Y2));
                    measurement.SideDropPhoto.YDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    measurement.SideDropPhoto.YDiameterInPixels = 0;
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    0,
                    measurement.SideDropPhoto.YDiameterInPixels / sideReferencePixelsInMillimeter,
                    measurement.SideDropPhoto.ZDiameterInPixels / sideReferencePixelsInMillimeter,
                    measurement.Drop);
            }
            else if (measurement.FrontDropPhotoId != null && measurement.SideDropPhotoId == null)
            {
                if (measurement.FrontDropPhoto.SimpleHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.X1),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.X2),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleHorizontalLine.Y2));
                    measurement.FrontDropPhoto.XDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    measurement.FrontDropPhoto.XDiameterInPixels = 0;
                }

                if (measurement.FrontDropPhoto.SimpleVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.X1),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.X2),
                        Convert.ToInt32(measurement.FrontDropPhoto.SimpleVerticalLine.Y2));
                    measurement.FrontDropPhoto.YDiameterInPixels =
                        LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    measurement.FrontDropPhoto.YDiameterInPixels = 0;
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    measurement.FrontDropPhoto.XDiameterInPixels / frontReferencePixelsInMillimeter,
                    measurement.FrontDropPhoto.YDiameterInPixels / frontReferencePixelsInMillimeter,
                    0,
                    measurement.Drop);
            }

            return measurement;
        }
    }
}