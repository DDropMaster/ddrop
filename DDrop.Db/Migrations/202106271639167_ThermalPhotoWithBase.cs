namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThermalPhotoWithBase : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.ThermalPhotos", "PhotoId", "dbo.BasePhotos", "PhotoId");
            DropColumn("dbo.ThermalPhotos", "Name");
            DropColumn("dbo.ThermalPhotos", "Content");
            DropColumn("dbo.ThermalPhotos", "AddedDate");
            DropColumn("dbo.ThermalPhotos", "CreationDateTime");
            DropColumn("dbo.ThermalPhotos", "PhotoType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ThermalPhotos", "PhotoType", c => c.String());
            AddColumn("dbo.ThermalPhotos", "CreationDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.ThermalPhotos", "AddedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ThermalPhotos", "Content", c => c.Binary());
            AddColumn("dbo.ThermalPhotos", "Name", c => c.String());
            DropForeignKey("dbo.ThermalPhotos", "PhotoId", "dbo.BasePhotos");
        }
    }
}
