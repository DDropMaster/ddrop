namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LessFieldsForPlot : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Plots", "XTitle");
            DropColumn("dbo.Plots", "YTitle");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Plots", "YTitle", c => c.String());
            AddColumn("dbo.Plots", "XTitle", c => c.String());
        }
    }
}
