namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentCommonTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DbComments",
                c => new
                    {
                        CommentId = c.Guid(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.CommentId);
            
            AddColumn("dbo.DropPhotos", "CommentId", c => c.Guid());
            AddColumn("dbo.Measurements", "CommentId", c => c.Guid());
            AddColumn("dbo.Series", "CommentId", c => c.Guid());
            AddColumn("dbo.ThermalPhotos", "CommentId", c => c.Guid());
            CreateIndex("dbo.DropPhotos", "CommentId");
            CreateIndex("dbo.Measurements", "CommentId");
            CreateIndex("dbo.Series", "CommentId");
            CreateIndex("dbo.ThermalPhotos", "CommentId");
            AddForeignKey("dbo.DropPhotos", "CommentId", "dbo.DbComments", "CommentId");
            AddForeignKey("dbo.Measurements", "CommentId", "dbo.DbComments", "CommentId");
            AddForeignKey("dbo.Series", "CommentId", "dbo.DbComments", "CommentId");
            AddForeignKey("dbo.ThermalPhotos", "CommentId", "dbo.DbComments", "CommentId");
            DropColumn("dbo.DropPhotos", "Comment");
            DropColumn("dbo.Measurements", "Comment");
            DropColumn("dbo.Series", "Comment");
            DropColumn("dbo.ThermalPhotos", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ThermalPhotos", "Comment", c => c.String());
            AddColumn("dbo.Series", "Comment", c => c.String());
            AddColumn("dbo.Measurements", "Comment", c => c.String());
            AddColumn("dbo.DropPhotos", "Comment", c => c.String());
            DropForeignKey("dbo.ThermalPhotos", "CommentId", "dbo.DbComments");
            DropForeignKey("dbo.Series", "CommentId", "dbo.DbComments");
            DropForeignKey("dbo.Measurements", "CommentId", "dbo.DbComments");
            DropForeignKey("dbo.DropPhotos", "CommentId", "dbo.DbComments");
            DropIndex("dbo.ThermalPhotos", new[] { "CommentId" });
            DropIndex("dbo.Series", new[] { "CommentId" });
            DropIndex("dbo.Measurements", new[] { "CommentId" });
            DropIndex("dbo.DropPhotos", new[] { "CommentId" });
            DropColumn("dbo.ThermalPhotos", "CommentId");
            DropColumn("dbo.Series", "CommentId");
            DropColumn("dbo.Measurements", "CommentId");
            DropColumn("dbo.DropPhotos", "CommentId");
            DropTable("dbo.DbComments");
        }
    }
}
