namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseDropPhotoTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasePhotos",
                c => new
                    {
                        PhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        Content = c.Binary(),
                        AddedDate = c.DateTime(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        PhotoType = c.String(),
                    })
                .PrimaryKey(t => t.PhotoId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BasePhotos");
        }
    }
}
