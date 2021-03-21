namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlotDimensionlessSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plots", "Settings", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plots", "Settings");
        }
    }
}
