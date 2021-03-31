namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropForeignKeyReferencePhoto : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReferencePhotos", "ReferencePhotoId", "dbo.Series");
        }
        
        public override void Down()
        {
            AddForeignKey("dbo.ReferencePhotos", "ReferencePhotoId", "dbo.Series");
        }
    }
}
