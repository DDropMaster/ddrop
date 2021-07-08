using System.Collections.Generic;

namespace DDrop.BL.ExcelOperations.Models
{
    public class ExcelReport
    {
        public string FileName { get; private set; }
        public bool Dimensionless { get; private set; }
        public List<BE.Models.Series> Series { get; private set; }
        public string UserFirstName { get; private set; }
        public string UserLastName { get; private set; }
        public string UserEmail { get; private set; }
        public List<BE.Models.Plot> UserPlots { get; private set; }

        public ExcelReport(string fileName, bool dimensionless, List<BE.Models.Series> series, string userFirstName, string userLastName, string userEmail, List<BE.Models.Plot> userPlots)
        {
            FileName = fileName;
            Dimensionless = dimensionless;
            Series = series;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            UserEmail = userEmail;
            UserPlots = userPlots;
        }
    }
}
