namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasurementOrder : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Measurements", "PhotoOrderInSeries", "MeasurementOrderInSeries");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Measurements", "MeasurementOrderInSeries", "PhotoOrderInSeries");
        }
    }
}
