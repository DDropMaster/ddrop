namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeriesSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "Settings", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Series", "Settings");
        }
    }
}
