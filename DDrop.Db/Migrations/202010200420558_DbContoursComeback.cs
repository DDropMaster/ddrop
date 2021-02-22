namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbContoursComeback : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE Measurements
                  SET ContourTemp = NULL"
            );
        }
        
        public override void Down()
        {
            Sql(@"UPDATE Measurements
                SET ContourTemp =
                   (
                   SELECT
                      JSON_QUERY('{""Ksize"":9,""Treshold1"":50,""Treshold2"":100,""Size1"":100,""Size2"":250}') as CalculationParameters,
	                  'CalculateWithCSharp' as CalculationProvider,
                      (SELECT X1, Y1, X2, Y2 FROM SimpleLines WHERE ContourId = dc.ContourId FOR JSON AUTO) AS SimpleLines
                   FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                   ) 
                FROM DbContours dc INNER JOIN [dbo].Measurements
                ON dc.ContourId = [dbo].Measurements.MeasurementId"
            );
        }
    }
}
