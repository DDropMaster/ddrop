using AutoMapper;
using DDrop.BE.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.Enums;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class RegionOfInterestResolver : IValueResolver<Series, SeriesView, ObservableCollection<TypedRectangleView>>
    {
        public ObservableCollection<TypedRectangleView> Resolve(Series source, SeriesView destination, ObservableCollection<TypedRectangleView> destMember,
            ResolutionContext context)
        {
            if (source.RegionOfInterest != null)
            {
                ObservableCollection<TypedRectangleView> regionOfInterest =
                    new ObservableCollection<TypedRectangleView>();

                for (int i = 0; i < source.RegionOfInterest.Count; i++)
                {
                    regionOfInterest.Add(new TypedRectangleView()
                    {
                        RegionOfInterest = new Rectangle()
                        {
                            Stroke = Brushes.Red,
                            StrokeThickness = 1,
                            Height = source.RegionOfInterest[i].Rectangle.Height,
                            Width = source.RegionOfInterest[i].Rectangle.Width,
                        },
                        Rectangle = source.RegionOfInterest[i].Rectangle,
                        PhotoType = (PhotoTypeView) source.RegionOfInterest[i].PhotoType
                    });

                    Canvas.SetLeft(regionOfInterest[i].RegionOfInterest, regionOfInterest[i].Rectangle.X);
                    Canvas.SetTop(regionOfInterest[i].RegionOfInterest, regionOfInterest[i].Rectangle.Y);
                }

                return regionOfInterest;
            }


            return null;
        }
    }
}