using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.Enums;
using DDrop.Models;

namespace DDrop.AutoMapper
{
    public class DropPhotoHorizontalLineResolver : IValueResolver<DropPhoto, DropPhotoView, ObservableCollection<TypedLineView>>
    {
        public ObservableCollection<TypedLineView> Resolve(DropPhoto source, DropPhotoView destination, ObservableCollection<TypedLineView> destMember, ResolutionContext context)
        {
            var resultingLines = new ObservableCollection<TypedLineView>();

            foreach (var line in source.SimpleLines)
            {
                resultingLines.Add(new TypedLineView
                {
                    LineType = (LineTypeView)line.LineType,
                    Line = new Line
                    {
                        X1 = line.X1,
                        X2 = line.X2,
                        Y1 = line.Y1,
                        Y2 = line.Y2,
                        Stroke = line.LineType == LineType.Vertical ? Brushes.Green : Brushes.DeepPink
                    }

                });
            }

            return resultingLines;
        }
    }
}
