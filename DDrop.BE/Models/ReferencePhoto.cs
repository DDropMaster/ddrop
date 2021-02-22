using System.Windows.Shapes;

namespace DDrop.BE.Models
{
    public class ReferencePhoto : BasePhoto
    {
        public Series Series { get; set; }

        public SimpleLine SimpleLine { get; set; }

        public int PixelsInMillimeter { get; set; }
    }
}