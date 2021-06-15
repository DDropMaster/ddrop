namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProcessedWasAMistake : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DropPhotos", "Processed");
            DropColumn("dbo.Measurements", "Processed");
            DropColumn("dbo.Series", "Processed");
            DropColumn("dbo.ThermalPhotos", "Processed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ThermalPhotos", "Processed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Series", "Processed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Measurements", "Processed", c => c.Boolean(nullable: false));
            AddColumn("dbo.DropPhotos", "Processed", c => c.Boolean(nullable: false));
        }
    }
}
