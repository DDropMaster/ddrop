namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoMeasurementContent : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Measurements", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Measurements", "Content", c => c.Binary());
        }
    }
}
