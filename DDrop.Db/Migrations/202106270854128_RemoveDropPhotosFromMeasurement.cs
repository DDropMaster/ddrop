namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDropPhotosFromMeasurement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos");
            DropForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos");
            DropIndex("dbo.Measurements", new[] { "SideDropPhotoId" });
            DropIndex("dbo.Measurements", new[] { "FrontDropPhotoId" });
            DropColumn("dbo.Measurements", "SideDropPhotoId");
            DropColumn("dbo.Measurements", "FrontDropPhotoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Measurements", "FrontDropPhotoId", c => c.Guid());
            AddColumn("dbo.Measurements", "SideDropPhotoId", c => c.Guid());
            CreateIndex("dbo.Measurements", "FrontDropPhotoId");
            CreateIndex("dbo.Measurements", "SideDropPhotoId");
            AddForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos", "PhotoId");
            AddForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos", "PhotoId");
        }
    }
}
