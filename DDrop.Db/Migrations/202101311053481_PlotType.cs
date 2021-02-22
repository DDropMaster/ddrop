namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlotType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plots", "PlotType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plots", "PlotType");
        }
    }
}
