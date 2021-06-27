namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropPhotosAsCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropPhotos", "MeasurementId", c => c.Guid(nullable: false));
            Sql(@"Update dbo.DropPhotos
                  SET MeasurementId = dbo.Measurements.MeasurementId
                  FROM dbo.Measurements                    
                  INNER JOIN dbo.DropPhotos on PhotoId = dbo.Measurements.FrontDropPhotoId");
            CreateIndex("dbo.DropPhotos", "MeasurementId");
            AddForeignKey("dbo.DropPhotos", "MeasurementId", "dbo.Measurements", "MeasurementId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DropPhotos", "MeasurementId", "dbo.Measurements");
            DropIndex("dbo.DropPhotos", new[] { "MeasurementId" });
            DropColumn("dbo.DropPhotos", "MeasurementId");
        }
    }
}
