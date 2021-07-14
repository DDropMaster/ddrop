using AutoMapper;
using DDrop.Enums;
using DDrop.Models;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DDrop.AutoMapper
{
    public class SimpleLineViewToLineConverter : ITypeConverter<SimpleLineView, Line>
    {
        public Line Convert(SimpleLineView source, Line destination, ResolutionContext context)
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
