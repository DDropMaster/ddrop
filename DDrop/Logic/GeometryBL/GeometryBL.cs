using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Shapes;
using DDrop.Controls.PixelDrawer;
using DDrop.Enums;
using DDrop.Models;
using Brushes = System.Windows.Media.Brushes;

namespace DDrop.Logic.GeometryBL
{
    public class GeometryBL : IGeometryBL
    {
        public void PrepareLines(DropPhotoView selectedPhoto, out ObservableCollection<TypedLineView> lines, bool showLinesOnPreview)
        {
            lines = new ObservableCollection<TypedLineView>();

            if (showLinesOnPreview)
            {
                foreach (var line in selectedPhoto.Lines)
                {
                    lines.Add(new TypedLineView
                    {
                        LineType = line.LineType,
                        Line = line.Line
                    });
                }
            }
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

            var horizontalSimpleLine = dropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == LineTypeView.Horizontal);

            if (horizontalSimpleLine == null)
            {
                dropPhoto.SimpleLines.Add(new SimpleLineView { LineType = LineTypeView.Horizontal });
                horizontalSimpleLine = dropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == LineTypeView.Horizontal);
            }

            horizontalSimpleLine.X1 = simpleHorizontalDiameter.X1;
            horizontalSimpleLine.X2 = simpleHorizontalDiameter.X2;
            horizontalSimpleLine.Y1 = simpleHorizontalDiameter.Y1;
            horizontalSimpleLine.Y2 = simpleHorizontalDiameter.Y1;

            var horizontalLine = dropPhoto.Lines.FirstOrDefault(x => x.LineType == LineTypeView.Horizontal);

            if (horizontalLine == null)
            {
                dropPhoto.Lines.Add(new TypedLineView { LineType = LineTypeView.Horizontal, Line = new Line() });
                horizontalLine = dropPhoto.Lines.FirstOrDefault(x => x.LineType == LineTypeView.Horizontal);
            }

            horizontalLine.Line.X1 = simpleHorizontalDiameter.X1;
            horizontalLine.Line.X2 = simpleHorizontalDiameter.X2;
            horizontalLine.Line.Y1 = simpleHorizontalDiameter.Y1;
            horizontalLine.Line.Y2 = simpleHorizontalDiameter.Y1;
            horizontalLine.Line.StrokeThickness = 2;
            horizontalLine.Line.Stroke = Brushes.DeepPink;

            var verticalSimpleLine = dropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == LineTypeView.Vertical);

            if (verticalSimpleLine == null)
            {
                dropPhoto.SimpleLines.Add(new SimpleLineView { LineType = LineTypeView.Vertical });
                verticalSimpleLine = dropPhoto.SimpleLines.FirstOrDefault(x => x.LineType == LineTypeView.Vertical);
            }

            verticalSimpleLine.X1 = simpleVerticalDiameter.X1;
            verticalSimpleLine.X2 = simpleVerticalDiameter.X1;
            verticalSimpleLine.Y1 = simpleVerticalDiameter.Y1;
            verticalSimpleLine.Y2 = simpleVerticalDiameter.Y2;

            var verticalLine = dropPhoto.Lines.FirstOrDefault(x => x.LineType == LineTypeView.Vertical);

            if (verticalLine == null)
            {
                dropPhoto.Lines.Add(new TypedLineView { LineType = LineTypeView.Vertical, Line = new Line() });
                verticalLine = dropPhoto.Lines.FirstOrDefault(x => x.LineType == LineTypeView.Vertical);
            }

            verticalLine.Line.X1 = simpleVerticalDiameter.X1;
            verticalLine.Line.X2 = simpleVerticalDiameter.X1;
            verticalLine.Line.Y1 = simpleVerticalDiameter.Y1;
            verticalLine.Line.Y2 = simpleVerticalDiameter.Y2;
            verticalLine.Line.StrokeThickness = 2;
            verticalLine.Line.Stroke = Brushes.Green;
        }

        public void PrepareContour(DropPhotoView selectedPhoto, out ObservableCollection<Line> contour, bool showContourOnPreview)
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
    }
}