namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropTemperature : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Drops", "Temperature", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Drops", "Temperature");
        }
    }
}
