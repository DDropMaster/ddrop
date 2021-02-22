namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropPhotos", "Comment", c => c.String());
            AddColumn("dbo.Measurements", "Comment", c => c.String());
            AddColumn("dbo.Series", "Comment", c => c.String());
            AddColumn("dbo.ThermalPhotos", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ThermalPhotos", "Comment");
            DropColumn("dbo.Series", "Comment");
            DropColumn("dbo.Measurements", "Comment");
            DropColumn("dbo.DropPhotos", "Comment");
        }
    }
}
