namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinesCollectionRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DropPhotos", "Lines", "SimpleLines");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DropPhotos", "SimpleLines", "Lines");
        }
    }
}
