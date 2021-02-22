namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinesToJson : DbMigration
    {
        public override void Up()
        {
            Sql(@"update Measurements
                  set HorizontalLine = (
                  SELECT X1, Y1, X2, Y2 
                  FROM SimpleLines 
                  WHERE SimpleLineId = m.SimpleHorizontalLineId FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER)
                  FROM Measurements m inner join SimpleLines
                  on SimpleHorizontalLineId = SimpleLineId"
            );

            Sql(@"update Measurements
                  set VerticalLine = (
                  SELECT X1, Y1, X2, Y2 
                  FROM SimpleLines 
                  WHERE SimpleLineId = m.SimpleVerticalLineId FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER)
                  FROM Measurements m inner join SimpleLines
                  on SimpleVerticalLineId = SimpleLineId"
            );
        }
        
        public override void Down()
        {
            Sql(@"UPDATE Measurements
                  SET HorizontalLine = NULL"
            );

            Sql(@"UPDATE Measurements
                  SET VerticalLine = NULL"
            );
        }
    }
}
