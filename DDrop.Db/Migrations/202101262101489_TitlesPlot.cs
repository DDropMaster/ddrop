namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TitlesPlot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plots", "XTitle", c => c.String());
            AddColumn("dbo.Plots", "YTitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plots", "YTitle");
            DropColumn("dbo.Plots", "XTitle");
        }
    }
}
