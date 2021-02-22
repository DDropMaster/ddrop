namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceLinesToJson : DbMigration
    {
        public override void Up()
        {
            Sql(@"update ReferencePhotos
                  set ReferenceLine = (
                  SELECT X1, Y1, X2, Y2 
                  FROM SimpleLines 
                  WHERE SimpleLineId = m.SimpleReferencePhotoLineId FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER)
                  FROM ReferencePhotos m inner join SimpleLines
                  on SimpleReferencePhotoLineId = SimpleLineId");
        }
        
        public override void Down()
        {
            Sql(@"UPDATE ReferencePhotos
                  SET ReferenceLine = NULL"
            );
        }
    }
}
