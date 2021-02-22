namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Plots : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Plots",
                c => new
                    {
                        PlotId = c.Guid(nullable: false),
                        Name = c.String(),
                        Points = c.String(),
                        CurrentUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.PlotId)
                .ForeignKey("dbo.Users", t => t.CurrentUserId, cascadeDelete: true)
                .Index(t => t.CurrentUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Plots", "CurrentUserId", "dbo.Users");
            DropIndex("dbo.Plots", new[] { "CurrentUserId" });
            DropTable("dbo.Plots");
        }
    }
}
