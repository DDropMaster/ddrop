namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveWeirdColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Series", "UseCreationDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Series", "UseCreationDateTime", c => c.Boolean(nullable: false));
        }
    }
}
