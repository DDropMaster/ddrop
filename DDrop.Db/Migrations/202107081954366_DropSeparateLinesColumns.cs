namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropSeparateLinesColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DropPhotos", "HorizontalLine");
            DropColumn("dbo.DropPhotos", "VerticalLine");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DropPhotos", "VerticalLine", c => c.String());
            AddColumn("dbo.DropPhotos", "HorizontalLine", c => c.String());
        }
    }
}
