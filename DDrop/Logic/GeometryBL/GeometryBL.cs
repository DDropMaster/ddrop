using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;
using DDrop.Controls.PixelDrawer;
using DDrop.Enums;
using DDrop.Models;
using Brushes = System.Windows.Media.Brushes;

namespace DDrop.Logic.GeometryBL
{
    public class GeometryBL : IGeometryBL
    {
        public void PrepareLines(DropPhotoView selectedPhoto, out Line horizontalLine, out Line verticalLine,
            bool showLinesOnPreview)
        {
            if (selectedPhoto.HorizontalLine != null && showLinesOnPreview)
                horizontalLine = new Line
                {
                    X1 = selectedPhoto.HorizontalLine.X1,
                    X2 = selectedPhoto.HorizontalLine.X2,
                    Y1 = selectedPhoto.HorizontalLine.Y1,
                    Y2 = selectedPhoto.HorizontalLine.Y2,
                    StrokeThickness = 2,
                    Stroke = Brushes.DeepPink
                };
            else
                horizontalLine = null;

            if (selectedPhoto.VerticalLine != null && showLinesOnPreview)
                verticalLine = new Line
                {
                    X1 = selectedPhoto.VerticalLine.X1,
                    X2 = selectedPhoto.VerticalLine.X2,
                    Y1 = selectedPhoto.VerticalLine.Y1,
                    Y2 = selectedPhoto.VerticalLine.Y2,
                    StrokeThickness = 2,
                    Stroke = Brushes.Green
                };
            else
                verticalLine = null;
        }

        public void CreateDiameters(DropPhotoView dropPhoto, Point[] points)
        {
            var biggestHorizontalDistance = 0;
            var biggestVerticalDistance = 0;
            var simpleHorizontalDiameter = new SimpleLineView();
            var simpleVerticalDiameter = new SimpleLineView();

            for (var i = 0; i < points.Length; i++)
            for (var j = i + 1; j < points.Length - 1; j++)
            {
                var currentHorizontalDistance = Math.Abs(points[i].X - points[j].X);
                if (currentHorizontalDistance > biggestHorizontalDistance)
                {
                    biggestHorizontalDistance = currentHorizontalDistance;

                    simpleHorizontalDiameter.X1 = points[i].X;
                    simpleHorizontalDiameter.Y1 = points[i].Y;
                    simpleHorizontalDiameter.X2 = points[j].X;
                    simpleHorizontalDiameter.Y2 = points[j].Y;
                }

                var currentVerticalDistance = Math.Abs(points[i].Y - points[j].Y);
                if (currentVerticalDistance > biggestVerticalDistance)
                {
                    biggestVerticalDistance = currentVerticalDistance;

                    simpleVerticalDiameter.X1 = points[i].X;
                    simpleVerticalDiameter.Y1 = points[i].Y;
                    simpleVerticalDiameter.X2 = points[j].X;
                    simpleVerticalDiameter.Y2 = points[j].Y;
                }
            }

            if (dropPhoto.SimpleHorizontalLine == null)
                dropPhoto.SimpleHorizontalLine = new SimpleLineView();

            dropPhoto.SimpleHorizontalLine.X1 = simpleHorizontalDiameter.X1;
            dropPhoto.SimpleHorizontalLine.X2 = simpleHorizontalDiameter.X2;
            dropPhoto.SimpleHorizontalLine.Y1 = simpleHorizontalDiameter.Y1;
            dropPhoto.SimpleHorizontalLine.Y2 = simpleHorizontalDiameter.Y1;

            if (dropPhoto.HorizontalLine == null)
                dropPhoto.HorizontalLine = new Line();

            dropPhoto.HorizontalLine.X1 = simpleHorizontalDiameter.X1;
            dropPhoto.HorizontalLine.X2 = simpleHorizontalDiameter.X2;
            dropPhoto.HorizontalLine.Y1 = simpleHorizontalDiameter.Y1;
            dropPhoto.HorizontalLine.Y2 = simpleHorizontalDiameter.Y1;
            dropPhoto.HorizontalLine.StrokeThickness = 2;
            dropPhoto.HorizontalLine.Stroke = Brushes.DeepPink;

            if (dropPhoto.SimpleVerticalLine == null)
                dropPhoto.SimpleVerticalLine = new SimpleLineView();

            dropPhoto.SimpleVerticalLine.X1 = simpleVerticalDiameter.X1;
            dropPhoto.SimpleVerticalLine.X2 = simpleVerticalDiameter.X1;
            dropPhoto.SimpleVerticalLine.Y1 = simpleVerticalDiameter.Y1;
            dropPhoto.SimpleVerticalLine.Y2 = simpleVerticalDiameter.Y2;

            if (dropPhoto.VerticalLine == null)
                dropPhoto.VerticalLine = new Line();

            dropPhoto.VerticalLine.X1 = simpleVerticalDiameter.X1;
            dropPhoto.VerticalLine.X2 = simpleVerticalDiameter.X1;
            dropPhoto.VerticalLine.Y1 = simpleVerticalDiameter.Y1;
            dropPhoto.VerticalLine.Y2 = simpleVerticalDiameter.Y2;
            dropPhoto.VerticalLine.StrokeThickness = 2;
            dropPhoto.VerticalLine.Stroke = Brushes.Green;
        }

        public void RestoreOriginalLines(DropPhotoView dropPhoto, DropPhotoView storedPhoto, Canvas canvas)
        {
            if (storedPhoto.SimpleHorizontalLine != null)
            {
                dropPhoto.SimpleHorizontalLine.X1 = storedPhoto.SimpleHorizontalLine.X1;
                dropPhoto.SimpleHorizontalLine.X2 = storedPhoto.SimpleHorizontalLine.X2;
                dropPhoto.SimpleHorizontalLine.Y1 = storedPhoto.SimpleHorizontalLine.Y1;
                dropPhoto.SimpleHorizontalLine.Y2 = storedPhoto.SimpleHorizontalLine.Y2;

                dropPhoto.HorizontalLine.X1 = dropPhoto.SimpleHorizontalLine.X1;
                dropPhoto.HorizontalLine.X2 = dropPhoto.SimpleHorizontalLine.X2;
                dropPhoto.HorizontalLine.Y1 = dropPhoto.SimpleHorizontalLine.Y1;
                dropPhoto.HorizontalLine.Y2 = dropPhoto.SimpleHorizontalLine.Y2;
            }
            else
            {
                canvas.Children.Remove(dropPhoto.HorizontalLine);
                dropPhoto.SimpleHorizontalLine = null;
                dropPhoto.HorizontalLine = null;
            }

            if (storedPhoto.SimpleVerticalLine != null)
            {
                dropPhoto.SimpleVerticalLine.X1 = storedPhoto.SimpleVerticalLine.X1;
                dropPhoto.SimpleVerticalLine.X2 = storedPhoto.SimpleVerticalLine.X2;
                dropPhoto.SimpleVerticalLine.Y1 = storedPhoto.SimpleVerticalLine.Y1;
                dropPhoto.SimpleVerticalLine.Y2 = storedPhoto.SimpleVerticalLine.Y2;

                dropPhoto.VerticalLine.X1 = dropPhoto.SimpleVerticalLine.X1;
                dropPhoto.VerticalLine.X2 = dropPhoto.SimpleVerticalLine.X2;
                dropPhoto.VerticalLine.Y1 = dropPhoto.SimpleVerticalLine.Y1;
                dropPhoto.VerticalLine.Y2 = dropPhoto.SimpleVerticalLine.Y2;
            }
            else
            {
                canvas.Children.Remove(dropPhoto.VerticalLine);
                dropPhoto.SimpleVerticalLine = null;
                dropPhoto.VerticalLine = null;
            }
        }

        public void PrepareContour(DropPhotoView selectedPhoto, out ObservableCollection<Line> contour,
            bool showContourOnPreview)
        {
            if (selectedPhoto.Contour?.Lines != null && showContourOnPreview)
            {
                contour = new ObservableCollection<Line>();

                foreach (var item in selectedPhoto.Contour.Lines)
                {
                    var lineForAdd = new Line
                    {
                        X1 = item.X1,
                        X2 = item.X2,
                        Y1 = item.Y1,
                        Y2 = item.Y2,
                        Stroke = item.Stroke,
                        Fill = item.Fill
                    };

                    contour.Add(lineForAdd);
                }
            }
            else
            {
                contour = null;
            }
        }

        public ContourView CreateContour(ContourView contour, Point[] points,
            CalculationVariantsView calculationVariant, AutoCalculationParametersView parameters, ContourView currentContour, PixelDrawer imgCurrent)
        {
            if (contour == null)
            {
                contour = new ContourView()
                {
                    ContourId = Guid.NewGuid(),
                    SimpleLines = new ObservableCollection<SimpleLineView>(),
                    Lines = new ObservableCollection<Line>()
                };
            }
            else
            {
                if (contour == currentContour)
                    foreach (var line in contour.Lines)
                        imgCurrent.CanDrawing.Children.Remove(line);

                contour.SimpleLines.Clear();
                contour.Lines.Clear();
            }

            contour.CalculationVariants = calculationVariant;
            contour.Parameters = parameters;

            for (var j = 0; j < points.Length; j++)
            {
                contour.SimpleLines.Add(new SimpleLineView()
                {
                    X1 = points[j].X,
                    X2 = j < points.Length - 1 ? points[j + 1].X : points[0].X,
                    Y1 = points[j].Y,
                    Y2 = j < points.Length - 1 ? points[j + 1].Y : points[0].Y
                });

                contour.Lines.Add(new Line
                {
                    X1 = points[j].X,
                    X2 = j < points.Length - 1 ? points[j + 1].X : points[0].X,
                    Y1 = points[j].Y,
                    Y2 = j < points.Length - 1 ? points[j + 1].Y : points[0].Y,
                    StrokeThickness = 2,
                    Stroke = Brushes.Red
                });
            }

            return contour;
        }

        public void StoreContour(ContourView contour, ContourView storeTo)
        {
            if (contour != null)
            {
                var storedContour = new ContourView()
                {
                    ContourId = contour.ContourId,
                    CalculationVariants = contour.CalculationVariants,
                    Parameters = new AutoCalculationParametersView()
                    {
                        Ksize = contour.Parameters.Ksize,
                        Size1 = contour.Parameters.Size1,
                        Size2 = contour.Parameters.Size2,
                        Treshold1 = contour.Parameters.Treshold1,
                        Treshold2 = contour.Parameters.Treshold2
                    },
                    SimpleLines = new ObservableCollection<SimpleLineView>()
                };

                foreach (var line in contour.SimpleLines)
                    storedContour.SimpleLines.Add(new SimpleLineView()
                    {
                        X1 = line.X1,
                        X2 = line.X2,
                        Y1 = line.Y1,
                        Y2 = line.Y2
                    });

                storeTo = storedContour;
            }
            else
            {
                storeTo = null;
            }
        }

        public void RestoreOriginalContour(DropPhotoView dropPhoto, DropPhotoView storedPhoto,
            Canvas canvas, Guid? currentMeasurementId)
        {
            if (storedPhoto.Contour != null && dropPhoto.Contour != null)
            {
                dropPhoto.Contour.ContourId = storedPhoto.Contour.ContourId;
                dropPhoto.Contour.CalculationVariants = storedPhoto.Contour.CalculationVariants;
                dropPhoto.Contour.Parameters = storedPhoto.Contour.Parameters;
                dropPhoto.Contour.SimpleLines = storedPhoto.Contour.SimpleLines;
            }
            else if (storedPhoto.Contour != null && dropPhoto.Contour == null)
            {
                dropPhoto.Contour = new ContourView()
                {
                    SimpleLines = new ObservableCollection<SimpleLineView>(),
                    ContourId = storedPhoto.Contour.ContourId,
                    CalculationVariants = storedPhoto.Contour.CalculationVariants,
                    Parameters = storedPhoto.Contour.Parameters
                };

                dropPhoto.Contour.SimpleLines = storedPhoto.Contour.SimpleLines;
            }
            else
            {
                if (dropPhoto.Contour != null && currentMeasurementId != null && dropPhoto.PhotoId == currentMeasurementId)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Remove(line);

                dropPhoto.Contour = null;
            }
        }
    }
}