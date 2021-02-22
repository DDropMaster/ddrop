namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThermalPhotos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThermalPhotos",
                c => new
                    {
                        PhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        Content = c.Binary(),
                        AddedDate = c.String(),
                        CreationDateTime = c.String(),
                        PhotoType = c.String(),
                    })
                .PrimaryKey(t => t.PhotoId)
                .ForeignKey("dbo.Measurements", t => t.PhotoId)
                .Index(t => t.PhotoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThermalPhotos", "PhotoId", "dbo.Measurements");
            DropIndex("dbo.ThermalPhotos", new[] { "PhotoId" });
            DropTable("dbo.ThermalPhotos");
        }
    }
}
