namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbStoredStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropPhotos", "Processed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Measurements", "Processed", c => c.Boolean(nullable: false));
            AddColumn("dbo.ThermalPhotos", "Processed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ThermalPhotos", "Processed");
            DropColumn("dbo.Measurements", "Processed");
            DropColumn("dbo.DropPhotos", "Processed");
        }
    }
}
