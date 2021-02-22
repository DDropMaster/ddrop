namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropContourId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DropPhotos", "Contour_ContourId");
        }
        
        public override void Down()
        {
        }
    }
}
