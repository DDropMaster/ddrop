using System.Text.Json.Serialization;
using System.Windows.Shapes;
using DDrop.BE.Enums;

namespace DDrop.BE.Models
{
    public class TypedRectangle
    {
        public System.Drawing.Rectangle Rectangle { get; set; }

        public PhotoType PhotoType { get; set; }
    }
}