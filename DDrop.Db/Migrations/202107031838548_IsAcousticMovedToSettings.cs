namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsAcousticMovedToSettings : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Series", "IsAcoustic");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Series", "IsAcoustic", c => c.Boolean(nullable: false));
        }
    }
}
