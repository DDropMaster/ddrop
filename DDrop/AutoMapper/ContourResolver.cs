using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class ContourResolver : IValueResolver<Contour, ContourView, ObservableCollection<Line>>
    {
        public ObservableCollection<Line> Resolve(Contour source, ContourView destination, ObservableCollection<Line> destMember,
            ResolutionContext context)
        {
            if (source.SimpleLines != null)
            {
                var lines = new ObservableCollection<Line>();
                foreach (var dbSimpleLine in source.SimpleLines)
                {
                    lines.Add(new Line
                    {
                        X1 = dbSimpleLine.X1,
                        X2 = dbSimpleLine.X2,
                        Y1 = dbSimpleLine.Y1,
                        Y2 = dbSimpleLine.Y2,
                        Stroke = Brushes.Red,
                        StrokeThickness = 2
                    });
                }

                return lines;
            }

            return null;
        }
    }
}
