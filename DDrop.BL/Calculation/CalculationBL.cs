using System;
using System.Drawing;
using System.Threading.Tasks;
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

        public async Task CalculateDropParameters(BE.Models.Measurement measurement, string pixelsInMillimeter, bool frontProcessed, bool sideProcessed)
        {
            var pixelsInMillimeterForCalculation = Convert.ToInt32(pixelsInMillimeter);

            if (measurement.FrontDropPhoto != null && frontProcessed && measurement.SideDropPhoto != null && sideProcessed)
            {
                var yDiameterInPixels = (measurement.FrontDropPhoto.YDiameterInPixels +
                                         measurement.SideDropPhoto.YDiameterInPixels) / 2;

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(pixelsInMillimeterForCalculation,
                    measurement.FrontDropPhoto.XDiameterInPixels,
                    yDiameterInPixels,
                    measurement.SideDropPhoto.ZDiameterInPixels, 
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
            else if ((measurement.FrontDropPhoto == null || !frontProcessed) && measurement.SideDropPhoto != null && sideProcessed)
            {
                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(pixelsInMillimeterForCalculation,
                    0,
                    measurement.SideDropPhoto.YDiameterInPixels,
                    measurement.SideDropPhoto.ZDiameterInPixels,
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
            else if (measurement.FrontDropPhoto != null && frontProcessed && (measurement.SideDropPhoto == null || !sideProcessed))
            {
                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(pixelsInMillimeterForCalculation,
                    measurement.FrontDropPhoto.XDiameterInPixels,
                    measurement.FrontDropPhoto.YDiameterInPixels,
                    0,
                    measurement.Drop);

                await _dropBl.UpdateDrop(measurement.Drop);
            }
        }

        public void ReCalculateAllParametersFromLines(BE.Models.Measurement measurement, string pixelsInMillimeterTextBox)
        {
            var pixelsInMillimeterForCalculation = Convert.ToInt32(pixelsInMillimeterTextBox);

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
                
                var yDiameterInPixels = (measurement.FrontDropPhoto.YDiameterInPixels +
                                         measurement.SideDropPhoto.YDiameterInPixels) / 2;

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(pixelsInMillimeterForCalculation,
                    measurement.FrontDropPhoto.XDiameterInPixels,
                    yDiameterInPixels,
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

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(pixelsInMillimeterForCalculation,
                    0,
                    measurement.SideDropPhoto.YDiameterInPixels,
                    measurement.SideDropPhoto.ZDiameterInPixels,
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

                DropletSizeCalculator.DropletSizeCalculator.PerformCalculation(pixelsInMillimeterForCalculation,
                    measurement.FrontDropPhoto.XDiameterInPixels,
                    measurement.FrontDropPhoto.YDiameterInPixels,
                    0,
                    measurement.Drop);
            }
        }
    }
}