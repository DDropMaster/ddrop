namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropMesDropPConstraints : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos");
            DropForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos");
            DropIndex("dbo.Measurements", new[] { "SideDropPhotoId" });
            DropIndex("dbo.Measurements", new[] { "FrontDropPhotoId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Measurements", "FrontDropPhotoId");
            CreateIndex("dbo.Measurements", "SideDropPhotoId");
            AddForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos", "PhotoId");
            AddForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos", "PhotoId");
        }
    }
}
