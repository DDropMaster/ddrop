namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinesCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropPhotos", "Lines", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DropPhotos", "Lines");
        }
    }
}
