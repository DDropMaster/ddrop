namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasurementsToDropPhotos : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE Measurements
                  SET FrontDropPhotoId = NEWID()");
        }
        
        public override void Down()
        {
            Sql(@"UPDATE Measurements
                  SET FrontDropPhotoId = NULL");
        }
    }
}
