namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AmbientTemperature : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurements", "AmbientTemperature", c => c.Double(nullable: false));
            DropColumn("dbo.Measurements", "Humidity");
            DropColumn("dbo.Measurements", "AtmospherePressure");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Measurements", "AtmospherePressure", c => c.Double(nullable: false));
            AddColumn("dbo.Measurements", "Humidity", c => c.Double(nullable: false));
            DropColumn("dbo.Measurements", "AmbientTemperature");
        }
    }
}
