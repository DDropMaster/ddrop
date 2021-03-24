namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleReferencePhotos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReferencePhotos", "PhotoId", "dbo.Series");
            DropIndex("dbo.ReferencePhotos", new[] { "PhotoId" });
            AddColumn("dbo.ReferencePhotos", "CurrentSeriesId", c => c.Guid());
            CreateIndex("dbo.ReferencePhotos", "CurrentSeriesId");
            AddForeignKey("dbo.ReferencePhotos", "CurrentSeriesId", "dbo.Series", "SeriesId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReferencePhotos", "CurrentSeriesId", "dbo.Series");
            DropIndex("dbo.ReferencePhotos", new[] { "CurrentSeriesId" });
            DropColumn("dbo.ReferencePhotos", "CurrentSeriesId");
            CreateIndex("dbo.ReferencePhotos", "PhotoId");
            AddForeignKey("dbo.ReferencePhotos", "PhotoId", "dbo.Series", "SeriesId");
        }
    }
}
