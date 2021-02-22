namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDropPhotoDrop : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DropPhotos", "DropId", "dbo.Drops");
            DropIndex("dbo.DropPhotos", new[] { "DropId" });
            DropColumn("dbo.DropPhotos", "DropId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DropPhotos", "DropId", c => c.Guid(nullable: false));
            CreateIndex("dbo.DropPhotos", "DropId");
            AddForeignKey("dbo.DropPhotos", "DropId", "dbo.Drops", "DropId", cascadeDelete: true);
        }
    }
}
