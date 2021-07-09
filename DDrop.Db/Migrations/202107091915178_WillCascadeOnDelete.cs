namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WillCascadeOnDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DropPhotos", "ContourId", "dbo.DbContours");
            DropForeignKey("dbo.Drops", "DropId", "dbo.Measurements");
            DropForeignKey("dbo.Substances", "SubstanceId", "dbo.Series");
            DropForeignKey("dbo.ThermalPhotos", "ContourId", "dbo.DbContours");
            AddForeignKey("dbo.DropPhotos", "ContourId", "dbo.DbContours", "ContourId", cascadeDelete: true);
            AddForeignKey("dbo.Drops", "DropId", "dbo.Measurements", "MeasurementId", cascadeDelete: true);
            AddForeignKey("dbo.Substances", "SubstanceId", "dbo.Series", "SeriesId", cascadeDelete: true);
            AddForeignKey("dbo.ThermalPhotos", "ContourId", "dbo.DbContours", "ContourId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThermalPhotos", "ContourId", "dbo.DbContours");
            DropForeignKey("dbo.Substances", "SubstanceId", "dbo.Series");
            DropForeignKey("dbo.Drops", "DropId", "dbo.Measurements");
            DropForeignKey("dbo.DropPhotos", "ContourId", "dbo.DbContours");
            AddForeignKey("dbo.ThermalPhotos", "ContourId", "dbo.DbContours", "ContourId");
            AddForeignKey("dbo.Substances", "SubstanceId", "dbo.Series", "SeriesId");
            AddForeignKey("dbo.Drops", "DropId", "dbo.Measurements", "MeasurementId");
            AddForeignKey("dbo.DropPhotos", "ContourId", "dbo.DbContours", "ContourId");
        }
    }
}
