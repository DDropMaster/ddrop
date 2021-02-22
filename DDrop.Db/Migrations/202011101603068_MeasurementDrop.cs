namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasurementDrop : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Drops", "DropId");
            AddForeignKey("dbo.Drops", "DropId", "dbo.Measurements", "MeasurementId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Drops", "DropId", "dbo.Measurements");
            DropIndex("dbo.Drops", new[] { "DropId" });
        }
    }
}
