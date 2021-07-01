namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferencePhotoWithBase : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ReferencePhotos", "PhotoId");
            AddForeignKey("dbo.ReferencePhotos", "PhotoId", "dbo.BasePhotos", "PhotoId");
            DropColumn("dbo.ReferencePhotos", "Name");
            DropColumn("dbo.ReferencePhotos", "Content");
            DropColumn("dbo.ReferencePhotos", "AddedDate");
            DropColumn("dbo.ReferencePhotos", "CreationDateTime");
            DropColumn("dbo.ReferencePhotos", "PhotoType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReferencePhotos", "PhotoType", c => c.String());
            AddColumn("dbo.ReferencePhotos", "CreationDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.ReferencePhotos", "AddedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ReferencePhotos", "Content", c => c.Binary());
            AddColumn("dbo.ReferencePhotos", "Name", c => c.String());
            DropForeignKey("dbo.ReferencePhotos", "PhotoId", "dbo.BasePhotos");
            DropIndex("dbo.ReferencePhotos", new[] { "PhotoId" });
        }
    }
}
