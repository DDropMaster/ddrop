namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsAcoustic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "IsAcoustic", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Series", "IsAcoustic");
        }
    }
}
