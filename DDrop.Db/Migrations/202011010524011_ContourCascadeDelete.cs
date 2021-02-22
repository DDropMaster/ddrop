namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContourCascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos");
            AddForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos", "PhotoId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos");
            AddForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos", "PhotoId");
        }
    }
}
