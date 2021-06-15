namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlotsLoadingTooDamnLong : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Plots", "SeriesId", "dbo.Series");
            DropIndex("dbo.Plots", new[] { "SeriesId" });
            AddColumn("dbo.Series", "ThermalPlot_PlotId", c => c.Guid());
            CreateIndex("dbo.Series", "ThermalPlot_PlotId");
            AddForeignKey("dbo.Series", "ThermalPlot_PlotId", "dbo.Plots", "PlotId");
            Sql(@"UPDATE dbo.Series
                  SET ThermalPlot_PlotId = PlotId
                  FROM dbo.Plots
                  INNER JOIN dbo.Series on dbo.Plots.SeriesId = dbo.Series.SeriesId");
            DropColumn("dbo.Plots", "SeriesId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Plots", "SeriesId", c => c.Guid());
            DropForeignKey("dbo.Series", "ThermalPlot_PlotId", "dbo.Plots");
            DropIndex("dbo.Series", new[] { "ThermalPlot_PlotId" });
            Sql(@"UPDATE dbo.Plots
                  SET dbo.Plots.SeriesId = ThermalPlot_PlotId
                  FROM dbo.Plots
                  INNER JOIN dbo.Series on dbo.Plots.PlotId = dbo.Series.ThermalPlot_PlotId");
            DropColumn("dbo.Series", "ThermalPlot_PlotId");
            CreateIndex("dbo.Plots", "SeriesId");
            AddForeignKey("dbo.Plots", "SeriesId", "dbo.Series", "SeriesId");
        }
    }
}
