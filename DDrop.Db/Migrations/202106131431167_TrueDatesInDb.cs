namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrueDatesInDb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DropPhotos", "AddedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DropPhotos", "CreationDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Measurements", "AddedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Measurements", "CreationDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Series", "AddedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReferencePhotos", "AddedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReferencePhotos", "CreationDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ThermalPhotos", "AddedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ThermalPhotos", "CreationDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DbLogEntries", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DbLogEntries", "Date", c => c.String());
            AlterColumn("dbo.ThermalPhotos", "CreationDateTime", c => c.String());
            AlterColumn("dbo.ThermalPhotos", "AddedDate", c => c.String());
            AlterColumn("dbo.ReferencePhotos", "CreationDateTime", c => c.String());
            AlterColumn("dbo.ReferencePhotos", "AddedDate", c => c.String());
            AlterColumn("dbo.Series", "AddedDate", c => c.String());
            AlterColumn("dbo.Measurements", "CreationDateTime", c => c.String());
            AlterColumn("dbo.Measurements", "AddedDate", c => c.String());
            AlterColumn("dbo.DropPhotos", "CreationDateTime", c => c.String());
            AlterColumn("dbo.DropPhotos", "AddedDate", c => c.String());
        }
    }
}
