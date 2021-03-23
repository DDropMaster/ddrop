using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class DropPhotoHorizontalLineResolver : IValueResolver<DropPhoto, DropPhotoView, Line>
    {
        public Line Resolve(DropPhoto source, DropPhotoView destination, Line destMember, ResolutionContext context)
        {
            if (source.SimpleHorizontalLine != null)
            {
                return new Line
                {
                    X1 = source.SimpleHorizontalLine.X1,
                    X2 = source.SimpleHorizontalLine.X2,
                    Y1 = source.SimpleHorizontalLine.Y1,
                    Y2 = source.SimpleHorizontalLine.Y2,
                    Stroke = Brushes.DeepPink
                };
                
            }


            return null;
        }
    }
}
