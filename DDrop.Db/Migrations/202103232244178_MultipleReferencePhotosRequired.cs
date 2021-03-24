namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleReferencePhotosRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReferencePhotos", "CurrentSeriesId", "dbo.Series");
            DropIndex("dbo.ReferencePhotos", new[] { "CurrentSeriesId" });
            AlterColumn("dbo.ReferencePhotos", "CurrentSeriesId", c => c.Guid(nullable: false));
            CreateIndex("dbo.ReferencePhotos", "CurrentSeriesId");
            AddForeignKey("dbo.ReferencePhotos", "CurrentSeriesId", "dbo.Series", "SeriesId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReferencePhotos", "CurrentSeriesId", "dbo.Series");
            DropIndex("dbo.ReferencePhotos", new[] { "CurrentSeriesId" });
            AlterColumn("dbo.ReferencePhotos", "CurrentSeriesId", c => c.Guid());
            CreateIndex("dbo.ReferencePhotos", "CurrentSeriesId");
            AddForeignKey("dbo.ReferencePhotos", "CurrentSeriesId", "dbo.Series", "SeriesId");
        }
    }
}
