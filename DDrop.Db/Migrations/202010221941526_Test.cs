namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos");
            DropIndex("dbo.DbContours", new[] { "ContourId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.DbContours", "ContourId");
            AddForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos", "PhotoId");
        }
    }
}
