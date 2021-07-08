using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Shapes;
using DDrop.Controls.PixelDrawer;
using DDrop.Enums;
using DDrop.Models;

namespace DDrop.Logic.GeometryBL
{
    public interface IGeometryBL
    {
        void PrepareLines(DropPhotoView selectedPhoto, out ObservableCollection<TypedLineView> lines, bool showLinesOnPreview);

        void CreateDiameters(DropPhotoView dropPhoto, Point[] points);

        void PrepareContour(DropPhotoView selectedPhoto, out ObservableCollection<Line> contour,
            bool showContourOnPreview);

        ContourView CreateContour(ContourView contour, Point[] points,
            CalculationVariantsView calculationVariant, AutoCalculationParametersView parameters, ContourView currentContour, PixelDrawer imgCurrent);

    }
}