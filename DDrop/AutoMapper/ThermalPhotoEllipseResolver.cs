using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class ThermalPhotoEllipseResolver : IValueResolver<ThermalPhoto, ThermalPhotoView, Ellipse>
    {
        public Ellipse Resolve(ThermalPhoto source, ThermalPhotoView destination, Ellipse destMember, ResolutionContext context)
        {
            return new Ellipse()
            {
                Stroke = System.Windows.Media.Brushes.DeepPink,
                Fill = System.Windows.Media.Brushes.DeepPink,
                Width = 1,
                Height = 1,
                StrokeThickness = 1,
            };
        }
    }
}