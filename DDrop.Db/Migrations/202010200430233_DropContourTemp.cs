namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropContourTemp : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Measurements", "ContourTemp");
            DropColumn("dbo.DropPhotos", "ContourTemp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DropPhotos", "ContourTemp", c => c.String());
            AddColumn("dbo.Measurements", "ContourTemp", c => c.String());
        }
    }
}
