namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteUnusedMeasurementsColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Measurements", "XDiameterInPixels");
            DropColumn("dbo.Measurements", "YDiameterInPixels");
            DropColumn("dbo.Measurements", "ZDiameterInPixels");
            DropColumn("dbo.Measurements", "HorizontalLine");
            DropColumn("dbo.Measurements", "VerticalLine");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Measurements", "VerticalLine", c => c.String());
            AddColumn("dbo.Measurements", "HorizontalLine", c => c.String());
            AddColumn("dbo.Measurements", "ZDiameterInPixels", c => c.Int(nullable: false));
            AddColumn("dbo.Measurements", "YDiameterInPixels", c => c.Int(nullable: false));
            AddColumn("dbo.Measurements", "XDiameterInPixels", c => c.Int(nullable: false));
        }
    }
}
