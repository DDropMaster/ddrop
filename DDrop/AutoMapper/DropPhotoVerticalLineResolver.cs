using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class DropPhotoVerticalLineResolver : IValueResolver<DropPhoto, DropPhotoView, Line>
    {
        public Line Resolve(DropPhoto source, DropPhotoView destination, Line destMember,
            ResolutionContext context)
        {
            if (source.SimpleVerticalLine != null)
            {

                return new Line
                {
                    X1 = source.SimpleVerticalLine.X1,
                    X2 = source.SimpleVerticalLine.X2,
                    Y1 = source.SimpleVerticalLine.Y1,
                    Y2 = source.SimpleVerticalLine.Y2,
                    Stroke = Brushes.Green
                };
            }

            return null;
        }
    }
}
