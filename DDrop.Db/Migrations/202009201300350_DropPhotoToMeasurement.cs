namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropPhotoToMeasurement : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER TABLE [dbo].[DbContours] DROP CONSTRAINT [FK_dbo.DbContours_dbo.DropPhotos_ContourId]");
            Sql(@"ALTER TABLE [dbo].[Drops] DROP CONSTRAINT [FK_dbo.Drops_dbo.DropPhotos_DropId]");
            
            Sql(@"ALTER TABLE [dbo].[DropPhotos] DROP CONSTRAINT [FK_dbo.DropPhotos_dbo.Series_CurrentSeriesId]");
            Sql(@"ALTER TABLE [dbo].[DropPhotos] DROP CONSTRAINT [FK_dbo.DropPhotos_dbo.SimpleLines_SimpleHorizontalLineId]");
            Sql(@"ALTER TABLE [dbo].[DropPhotos] DROP CONSTRAINT [FK_dbo.DropPhotos_dbo.SimpleLines_SimpleVerticalLineId]");

            Sql("ALTER TABLE [dbo].[DropPhotos] DROP CONSTRAINT [PK_dbo.DropPhotos]");
            RenameColumn("dbo.DropPhotos", "DropPhotoId", "MeasurementId");
            RenameTable("DropPhotos", "Measurements");
            AddPrimaryKey("Measurements", "MeasurementId", "PK_dbo.Measurements");

            AddForeignKey("DbContours", "ContourId", "Measurements", name: "FK_dbo.DbContours_dbo.Measurements_ContourId");
            AddForeignKey("Drops", "DropId", "Measurements", name: "FK_dbo.Drops_dbo.Measurements_DropId");

            AddForeignKey("Measurements", "CurrentSeriesId", "Series", name: "FK_dbo.Measurements_dbo.Series_CurrentSeriesId");
            AddForeignKey("Measurements", "SimpleHorizontalLineId", "SimpleLines", name: "FK_dbo.Measurements_dbo.SimpleLines_SimpleHorizontalLineId");
            AddForeignKey("Measurements", "SimpleVerticalLineId", "SimpleLines", name: "FK_dbo.Measurements_dbo.SimpleLines_SimpleVerticalLineId");
        }
        
        public override void Down()
        {
            Sql(@"ALTER TABLE [dbo].[DbContours] DROP CONSTRAINT [FK_dbo.DbContours_dbo.Measurements_ContourId]");
            Sql(@"ALTER TABLE [dbo].[Drops] DROP CONSTRAINT [FK_dbo.Drops_dbo.Measurements_DropId]");

            Sql(@"ALTER TABLE [dbo].[Measurements] DROP CONSTRAINT [FK_dbo.Measurements_dbo.Series_CurrentSeriesId]");
            Sql(@"ALTER TABLE [dbo].[Measurements] DROP CONSTRAINT [FK_dbo.Measurements_dbo.SimpleLines_SimpleHorizontalLineId]");
            Sql(@"ALTER TABLE [dbo].[Measurements] DROP CONSTRAINT [FK_dbo.Measurements_dbo.SimpleLines_SimpleVerticalLineId]");

            Sql("ALTER TABLE [dbo].[Measurements] DROP CONSTRAINT [PK_dbo.Measurements]");
            RenameTable("[Measurements]", "DropPhotos");
            RenameColumn("dbo.DropPhotos", "MeasurementId", "DropPhotoId");
            AddPrimaryKey("DropPhotos", "DropPhotoId", "PK_dbo.DropPhotos");

            AddForeignKey("DbContours", "ContourId", "DropPhotos", name: "FK_dbo.DbContours_dbo.DropPhotos_ContourId");
            AddForeignKey("Drops", "DropId", "DropPhotos", name: "FK_dbo.Drops_dbo.DropPhotos_DropId");

            AddForeignKey("DropPhotos", "CurrentSeriesId", "Series", name: "FK_dbo.DropPhotos_dbo.Series_CurrentSeriesId");
            AddForeignKey("DropPhotos", "SimpleHorizontalLineId", "SimpleLines", name: "FK_dbo.DropPhotos_dbo.SimpleLines_SimpleHorizontalLineId");
            AddForeignKey("DropPhotos", "SimpleVerticalLineId", "SimpleLines", name: "FK_dbo.DropPhotos_dbo.SimpleLines_SimpleVerticalLineId");
        }
    }
}
