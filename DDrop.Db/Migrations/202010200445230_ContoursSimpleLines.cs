namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContoursSimpleLines : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReferencePhotos", "SimpleReferencePhotoLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.Measurements", "SimpleHorizontalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.Measurements", "SimpleVerticalLineId", "dbo.SimpleLines");
            DropIndex("dbo.Measurements", new[] { "SimpleHorizontalLineId" });
            DropIndex("dbo.Measurements", new[] { "SimpleVerticalLineId" });
            DropIndex("dbo.ReferencePhotos", new[] { "SimpleReferencePhotoLineId" });
            AddColumn("dbo.DbContours", "ConnectedLines", c => c.String());
            DropColumn("dbo.Measurements", "SimpleHorizontalLineId");
            DropColumn("dbo.Measurements", "SimpleVerticalLineId");
            DropColumn("dbo.ReferencePhotos", "SimpleReferencePhotoLineId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReferencePhotos", "SimpleReferencePhotoLineId", c => c.Guid());
            AddColumn("dbo.Measurements", "SimpleVerticalLineId", c => c.Guid());
            AddColumn("dbo.Measurements", "SimpleHorizontalLineId", c => c.Guid());
            DropColumn("dbo.DbContours", "ConnectedLines");
            CreateIndex("dbo.ReferencePhotos", "SimpleReferencePhotoLineId");
            CreateIndex("dbo.Measurements", "SimpleVerticalLineId");
            CreateIndex("dbo.Measurements", "SimpleHorizontalLineId");
            AddForeignKey("dbo.Measurements", "SimpleVerticalLineId", "dbo.SimpleLines", "SimpleLineId");
            AddForeignKey("dbo.Measurements", "SimpleHorizontalLineId", "dbo.SimpleLines", "SimpleLineId");
            AddForeignKey("dbo.ReferencePhotos", "SimpleReferencePhotoLineId", "dbo.SimpleLines", "SimpleLineId");
        }
    }
}
