namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferencePhotos", "ReferenceLine", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReferencePhotos", "ReferenceLine");
        }
    }
}
