namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoMoreLinesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DropPhotos", "SimpleHorizontalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotos", "SimpleVerticalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.SimpleLines", "ContourId", "dbo.DbContours");
            DropIndex("dbo.DropPhotos", new[] { "SimpleHorizontalLineId" });
            DropIndex("dbo.DropPhotos", new[] { "SimpleVerticalLineId" });
            DropIndex("dbo.SimpleLines", new[] { "ContourId" });
            DropColumn("dbo.DropPhotos", "SimpleHorizontalLineId");
            DropColumn("dbo.DropPhotos", "SimpleVerticalLineId");
            DropTable("dbo.SimpleLines");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SimpleLines",
                c => new
                    {
                        SimpleLineId = c.Guid(nullable: false),
                        X1 = c.Double(nullable: false),
                        Y1 = c.Double(nullable: false),
                        X2 = c.Double(nullable: false),
                        Y2 = c.Double(nullable: false),
                        ContourId = c.Guid(),
                    })
                .PrimaryKey(t => t.SimpleLineId);
            
            AddColumn("dbo.DropPhotos", "SimpleVerticalLineId", c => c.Guid());
            AddColumn("dbo.DropPhotos", "SimpleHorizontalLineId", c => c.Guid());
            CreateIndex("dbo.SimpleLines", "ContourId");
            CreateIndex("dbo.DropPhotos", "SimpleVerticalLineId");
            CreateIndex("dbo.DropPhotos", "SimpleHorizontalLineId");
            AddForeignKey("dbo.SimpleLines", "ContourId", "dbo.DbContours", "ContourId");
            AddForeignKey("dbo.DropPhotos", "SimpleVerticalLineId", "dbo.SimpleLines", "SimpleLineId");
            AddForeignKey("dbo.DropPhotos", "SimpleHorizontalLineId", "dbo.SimpleLines", "SimpleLineId");
        }
    }
}
