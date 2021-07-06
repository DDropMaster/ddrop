using System.ComponentModel;

namespace DDrop.BL.ExcelOperations.Models
{
    class PlotToExcel
    {
        [DisplayName("X")]
        public double X { get; set; }

        [DisplayName("Y")]
        public double Y { get; set; }
    }
}
