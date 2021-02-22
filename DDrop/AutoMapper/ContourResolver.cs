using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Db.DbEntities;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.AutoMapper
{
    public class ContourResolver : IValueResolver<DbContour, Contour, ObservableCollection<Line>>
    {
        public ObservableCollection<Line> Resolve(DbContour source, Contour destination, ObservableCollection<Line> destMember,
            ResolutionContext context)
        {
            if (source.ConnectedLines != null)
            {
                var simpleLines = JsonSerializeProvider.DeserializeFromString<ObservableCollection<SimpleLine>>(source.ConnectedLines);

                var lines = new ObservableCollection<Line>();
                foreach (var dbSimpleLine in simpleLines)
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
