using System;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using DDrop.BE.Enums.Options;

namespace DDrop.BE.Models
{
    public class Contour
    {
        public Guid ContourId { get; set; }

        public ObservableCollection<SimpleLine> SimpleLines { get; set; }

        public ObservableCollection<Line> Lines { get; set; }

        public AutoCalculationParameters Parameters { get; set; }

        public CalculationVariants CalculationVariants { get; set; }
    }
}