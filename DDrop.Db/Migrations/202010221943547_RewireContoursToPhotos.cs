namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RewireContoursToPhotos : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DbContours", "ContourId");
            AddForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos", "PhotoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos");
            DropIndex("dbo.DbContours", new[] { "ContourId" });
        }
    }
}
