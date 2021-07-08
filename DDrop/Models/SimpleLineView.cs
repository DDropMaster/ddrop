using DDrop.Enums;

namespace DDrop.Models
{
    public class SimpleLineView
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public LineTypeView LineType {get; set;}
    }
}