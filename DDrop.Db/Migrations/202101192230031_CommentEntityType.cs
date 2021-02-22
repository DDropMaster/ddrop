namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentEntityType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbComments", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DbComments", "Type");
        }
    }
}
