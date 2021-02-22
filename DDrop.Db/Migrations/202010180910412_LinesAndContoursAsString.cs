namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveDataFromMeasurements : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurements", "ContourTemp", c => c.String());
            AddColumn("dbo.Measurements", "HorizontalLine", c => c.String());
            AddColumn("dbo.Measurements", "VerticalLine", c => c.String());
            AddColumn("dbo.DropPhotos", "ContourTemp", c => c.String());
            AddColumn("dbo.DropPhotos", "HorizontalLine", c => c.String());
            AddColumn("dbo.DropPhotos", "VerticalLine", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DropPhotos", "VerticalLine");
            DropColumn("dbo.DropPhotos", "HorizontalLine");
            DropColumn("dbo.DropPhotos", "ContourTemp");
            DropColumn("dbo.Measurements", "VerticalLine");
            DropColumn("dbo.Measurements", "HorizontalLine");
            DropColumn("dbo.Measurements", "ContourTemp");
        }
    }
}
