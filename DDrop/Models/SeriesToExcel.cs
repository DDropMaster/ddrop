using System.ComponentModel;

namespace DDrop.Models
{
    public class SeriesToExcel
    {
        [DisplayName("Время, с")] 
        public double Time { get; set; }

        [DisplayName("Имя файла")] 
        public string Name { get; set; }

        [DisplayName("X диаметр, м")]
        public double XDiameterInMeters { get; set; }

        [DisplayName("Y диаметр, м")]
        public double YDiameterInMeters { get; set; }

        [DisplayName("Z диаметр, м")]
        public double ZDiameterInMeters { get; set; }

        [DisplayName("Радиус, м")] 
        public double RadiusInMeters { get; set; }

        [DisplayName("Объем, кубические метры")]
        public double VolumeInCubicalMeters { get; set; }

        [DisplayName("Температура")]
        public double Temperature { get; set; }
    }
}