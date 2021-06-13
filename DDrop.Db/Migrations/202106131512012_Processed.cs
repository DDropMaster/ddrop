namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Processed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "Processed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Series", "Processed");
        }
    }
}
