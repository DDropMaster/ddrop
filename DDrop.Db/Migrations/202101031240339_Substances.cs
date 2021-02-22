namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Substances : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Substances",
                c => new
                    {
                        SubstanceId = c.Guid(nullable: false),
                        CommonName = c.String(),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubstanceId)
                .ForeignKey("dbo.Series", t => t.SubstanceId)
                .Index(t => t.SubstanceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Substances", "SubstanceId", "dbo.Series");
            DropIndex("dbo.Substances", new[] { "SubstanceId" });
            DropTable("dbo.Substances");
        }
    }
}
