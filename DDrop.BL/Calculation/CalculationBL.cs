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

            if (frontDropPhoto?.PhotoId != null && frontDropPhoto?.PhotoId != Guid.Empty && sideDropPhoto?.PhotoId != null && sideDropPhoto?.PhotoId != Guid.Empty)
            {
                var frontHorizontalLine = frontDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Horizontal);

                if (frontHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(frontHorizontalLine.X1), Convert.ToInt32(frontHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(frontHorizontalLine.X2), Convert.ToInt32(frontHorizontalLine.Y2));

                    frontDropPhoto.XDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.XDiameterInPixels = 0;
                }

                var frontVerticalLine = frontDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Vertical);

                if (frontVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(frontVerticalLine.X1), Convert.ToInt32(frontVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(frontVerticalLine.X2), Convert.ToInt32(frontVerticalLine.Y2));

                    frontDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.YDiameterInPixels = 0;
                }

                var sideHorizontalLine = sideDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Horizontal);

                if (sideHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(sideHorizontalLine.X1), Convert.ToInt32(sideHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(sideHorizontalLine.X2), Convert.ToInt32(sideHorizontalLine.Y2));

                    sideDropPhoto.ZDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.ZDiameterInPixels = 0;
                }

                var sideVerticalLine = sideDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Vertical);

                if (sideVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(sideVerticalLine.X1), Convert.ToInt32(sideVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(sideVerticalLine.X2), Convert.ToInt32(sideVerticalLine.Y2));

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
            else if ((frontDropPhoto?.PhotoId == null || frontDropPhoto?.PhotoId == Guid.Empty) && sideDropPhoto?.PhotoId != null && sideDropPhoto?.PhotoId != Guid.Empty)
            {
                var sideHorizontalLine = sideDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Horizontal);

                if (sideHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(sideHorizontalLine.X1), Convert.ToInt32(sideHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(sideHorizontalLine.X2), Convert.ToInt32(sideHorizontalLine.Y2));

                    sideDropPhoto.ZDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.ZDiameterInPixels = 0;
                }

                var sideVerticalLine = sideDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Vertical);

                if (sideVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(sideVerticalLine.X1), Convert.ToInt32(sideVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(sideVerticalLine.X2), Convert.ToInt32(sideVerticalLine.Y2));

                    sideDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    sideDropPhoto.YDiameterInPixels = 0;
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    0,
                    sideDropPhoto.YDiameterInPixels / (double)sideReferencePixelsInMillimeter.Value,
                    sideDropPhoto.ZDiameterInPixels / (double)sideReferencePixelsInMillimeter.Value,
                    measurement.Drop);
            }
            else if (frontDropPhoto?.PhotoId != null && frontDropPhoto?.PhotoId != Guid.Empty && (sideDropPhoto?.PhotoId == null || sideDropPhoto?.PhotoId == Guid.Empty))
            {
                var frontHorizontalLine = frontDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Horizontal);

                if (frontHorizontalLine != null)
                {
                    var horizontalLineFirstPoint = new Point(Convert.ToInt32(frontHorizontalLine.X1), Convert.ToInt32(frontHorizontalLine.Y1));
                    var horizontalLineSecondPoint = new Point(Convert.ToInt32(frontHorizontalLine.X2), Convert.ToInt32(frontHorizontalLine.Y2));

                    frontDropPhoto.XDiameterInPixels = LineLengthHelper.GetPointsOnLine(horizontalLineFirstPoint, horizontalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.XDiameterInPixels = 0;
                }

                var frontVerticalLine = frontDropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == Enums.LineType.Vertical);

                if (frontVerticalLine != null)
                {
                    var verticalLineFirstPoint = new Point(Convert.ToInt32(frontVerticalLine.X1), Convert.ToInt32(frontVerticalLine.Y1));
                    var verticalLineSecondPoint = new Point(Convert.ToInt32(frontVerticalLine.X2), Convert.ToInt32(frontVerticalLine.Y2));

                    frontDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(verticalLineFirstPoint, verticalLineSecondPoint).Count;
                }
                else
                {
                    frontDropPhoto.YDiameterInPixels = 0;
                }

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(
                    frontDropPhoto.XDiameterInPixels / (double)frontReferencePixelsInMillimeter.Value,
                    frontDropPhoto.YDiameterInPixels / (double)frontReferencePixelsInMillimeter.Value,
                    0,
                    measurement.Drop);
            }

            return measurement;
        }
    }
}