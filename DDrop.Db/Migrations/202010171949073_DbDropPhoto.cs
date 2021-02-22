namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbDropPhoto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DropPhotos",
                c => new
                    {
                        PhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        XDiameterInPixels = c.Int(nullable: false),
                        YDiameterInPixels = c.Int(nullable: false),
                        ZDiameterInPixels = c.Int(nullable: false),
                        SimpleHorizontalLineId = c.Guid(),
                        SimpleVerticalLineId = c.Guid(),
                        Content = c.Binary(),
                        DropId = c.Guid(nullable: false),
                        AddedDate = c.String(),
                        CreationDateTime = c.String(),
                        PhotoType = c.String(),
                        Contour_ContourId = c.Guid(),
                    })
                .PrimaryKey(t => t.PhotoId)
                .ForeignKey("dbo.DbContours", t => t.Contour_ContourId)
                .ForeignKey("dbo.Drops", t => t.DropId, cascadeDelete: true)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleHorizontalLineId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleVerticalLineId)
                .Index(t => t.SimpleHorizontalLineId)
                .Index(t => t.SimpleVerticalLineId)
                .Index(t => t.DropId)
                .Index(t => t.Contour_ContourId);
            
            AddColumn("dbo.Measurements", "Humidity", c => c.Double(nullable: false));
            AddColumn("dbo.Measurements", "AtmospherePressure", c => c.Double(nullable: false));
            AddColumn("dbo.Measurements", "SideDropPhotoId", c => c.Guid());
            AddColumn("dbo.Measurements", "FrontDropPhotoId", c => c.Guid());
            CreateIndex("dbo.Measurements", "SideDropPhotoId");
            CreateIndex("dbo.Measurements", "FrontDropPhotoId");
            AddForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos", "PhotoId");
            AddForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos", "PhotoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Measurements", "SideDropPhotoId", "dbo.DropPhotos");
            DropForeignKey("dbo.Measurements", "FrontDropPhotoId", "dbo.DropPhotos");
            DropForeignKey("dbo.DropPhotos", "SimpleVerticalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotos", "SimpleHorizontalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotos", "DropId", "dbo.Drops");
            DropForeignKey("dbo.DropPhotos", "Contour_ContourId", "dbo.DbContours");
            DropIndex("dbo.DropPhotos", new[] { "Contour_ContourId" });
            DropIndex("dbo.DropPhotos", new[] { "DropId" });
            DropIndex("dbo.DropPhotos", new[] { "SimpleVerticalLineId" });
            DropIndex("dbo.DropPhotos", new[] { "SimpleHorizontalLineId" });
            DropIndex("dbo.Measurements", new[] { "FrontDropPhotoId" });
            DropIndex("dbo.Measurements", new[] { "SideDropPhotoId" });
            DropColumn("dbo.Measurements", "FrontDropPhotoId");
            DropColumn("dbo.Measurements", "SideDropPhotoId");
            DropColumn("dbo.Measurements", "AtmospherePressure");
            DropColumn("dbo.Measurements", "Humidity");
            DropTable("dbo.DropPhotos");
        }
    }
}
