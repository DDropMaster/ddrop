using System.ComponentModel;

namespace DDrop.Models
{
    class PlotToExcel
    {
        [DisplayName("X")]
        public double Time { get; set; }

        [DisplayName("Y")]
        public string Name { get; set; }
    }
}
