namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeriesThermalPlot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plots", "SeriesId", c => c.Guid());
            CreateIndex("dbo.Plots", "SeriesId");
            AddForeignKey("dbo.Plots", "SeriesId", "dbo.Series", "SeriesId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Plots", "SeriesId", "dbo.Series");
            DropIndex("dbo.Plots", new[] { "SeriesId" });
            DropColumn("dbo.Plots", "SeriesId");
        }
    }
}
