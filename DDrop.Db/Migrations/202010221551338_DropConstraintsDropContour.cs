namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropConstraintsDropContour : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Drops", "DropId", "dbo.Measurements");
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.Measurements");
            DropIndex("dbo.DbContours", new[] { "ContourId" });
            DropIndex("dbo.Drops", new[] { "DropId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Drops", "DropId");
            CreateIndex("dbo.DbContours", "ContourId");
            AddForeignKey("dbo.DbContours", "ContourId", "dbo.Measurements", "MeasurementId");
            AddForeignKey("dbo.Drops", "DropId", "dbo.Measurements", "MeasurementId");
        }
    }
}
