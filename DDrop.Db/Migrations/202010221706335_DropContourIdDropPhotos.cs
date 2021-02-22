namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropContourIdDropPhotos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DropPhotos", "Contour_ContourId", "dbo.DbContours");
            DropIndex("dbo.DropPhotos", new[] { "Contour_ContourId" });
        }
        
        public override void Down()
        {
        }
    }
}
