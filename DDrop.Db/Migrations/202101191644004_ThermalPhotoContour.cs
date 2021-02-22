namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThermalPhotoContour : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos");
            DropIndex("dbo.DbContours", new[] { "ContourId" });
            AddColumn("dbo.DropPhotos", "ContourId", c => c.Guid());
            AddColumn("dbo.ThermalPhotos", "ContourId", c => c.Guid());
            CreateIndex("dbo.DropPhotos", "ContourId");
            CreateIndex("dbo.ThermalPhotos", "ContourId");
            AddForeignKey("dbo.ThermalPhotos", "ContourId", "dbo.DbContours", "ContourId");
            AddForeignKey("dbo.DropPhotos", "ContourId", "dbo.DbContours", "ContourId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DropPhotos", "ContourId", "dbo.DbContours");
            DropForeignKey("dbo.ThermalPhotos", "ContourId", "dbo.DbContours");
            DropIndex("dbo.ThermalPhotos", new[] { "ContourId" });
            DropIndex("dbo.DropPhotos", new[] { "ContourId" });
            DropColumn("dbo.ThermalPhotos", "ContourId");
            DropColumn("dbo.DropPhotos", "ContourId");
            CreateIndex("dbo.DbContours", "ContourId");
            AddForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos", "PhotoId", cascadeDelete: true);
        }
    }
}
