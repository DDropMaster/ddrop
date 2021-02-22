namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReWireEverything : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Measurements", "SideDropPhotoId");
            CreateIndex("dbo.Measurements", "FrontDropPhotoId");
            AddForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos", "PhotoId");
            AddForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos", "PhotoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos");
            DropForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos");
            DropIndex("dbo.Measurements", new[] { "FrontDropPhotoId" });
            DropIndex("dbo.Measurements", new[] { "SideDropPhotoId" });
        }
    }
}
