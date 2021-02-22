namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContorLinesToString : DbMigration
    {
        public override void Up()
        {
            Sql(@"update DbContours
                  set ConnectedLines = (
                  SELECT X1, Y1, X2, Y2 
                  FROM SimpleLines 
                  WHERE ContourId = dc.ContourId FOR JSON AUTO)
                  FROM DbContours dc inner join SimpleLines
                  on dc.ContourId = SimpleLines.ContourId"
            );
        }
        
        public override void Down()
        {
            Sql(@"update DbContours
                  set ConnectedLines = NULL"
            );
        }
    }
}
