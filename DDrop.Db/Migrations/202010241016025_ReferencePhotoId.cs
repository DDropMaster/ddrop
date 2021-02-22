namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferencePhotoId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ReferencePhotos", name: "ReferencePhotoId", newName: "PhotoId");
            RenameIndex(table: "dbo.ReferencePhotos", name: "IX_ReferencePhotoId", newName: "IX_PhotoId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ReferencePhotos", name: "IX_PhotoId", newName: "IX_ReferencePhotoId");
            RenameColumn(table: "dbo.ReferencePhotos", name: "PhotoId", newName: "ReferencePhotoId");
        }
    }
}
