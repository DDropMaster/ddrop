using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Db.DbEntities;
using DDrop.Models;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.AutoMapper
{
    public class ReferencePhotoResolver : IValueResolver<ReferencePhoto, ReferencePhotoView,Line>
    {
        public Line Resolve(ReferencePhoto source, ReferencePhotoView destination, Line destMember, ResolutionContext context)
        {
            if (source.SimpleLine != null)
            {
                return new Line
                {
                    X1 = source.SimpleLine.X1,
                    X2 = source.SimpleLine.X2,
                    Y1 = source.SimpleLine.Y1,
                    Y2 = source.SimpleLine.Y2,
                    Stroke = Brushes.DeepPink
                };
            }


            return null;
        }
    }
}
