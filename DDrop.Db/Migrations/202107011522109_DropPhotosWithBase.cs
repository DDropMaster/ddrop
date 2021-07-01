namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropPhotosWithBase : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DropPhotos", "PhotoId");
            AddForeignKey("dbo.DropPhotos", "PhotoId", "dbo.BasePhotos", "PhotoId");
            DropColumn("dbo.DropPhotos", "Name");
            DropColumn("dbo.DropPhotos", "Content");
            DropColumn("dbo.DropPhotos", "AddedDate");
            DropColumn("dbo.DropPhotos", "CreationDateTime");
            DropColumn("dbo.DropPhotos", "PhotoType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DropPhotos", "PhotoType", c => c.String());
            AddColumn("dbo.DropPhotos", "CreationDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.DropPhotos", "AddedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DropPhotos", "Content", c => c.Binary());
            AddColumn("dbo.DropPhotos", "Name", c => c.String());
            DropForeignKey("dbo.DropPhotos", "PhotoId", "dbo.BasePhotos");
            DropIndex("dbo.DropPhotos", new[] { "PhotoId" });
        }
    }
}
