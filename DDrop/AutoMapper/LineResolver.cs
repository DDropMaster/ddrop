using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Enums;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class LineResolver : IValueResolver<SimpleLineView, TypedLineView, Line>
    {
        public Line Resolve(SimpleLineView source, TypedLineView destination, Line destMember, ResolutionContext context)
        {
            return new Line
            {
                X1 = source.X1,
                X2 = source.X2,
                Y1 = source.Y1,
                Y2 = source.Y2,
                Stroke = source.LineType == LineTypeView.Vertical ? Brushes.Green : Brushes.DeepPink
            };
        }
    }
}
