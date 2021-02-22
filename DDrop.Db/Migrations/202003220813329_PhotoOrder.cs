using System.Data.Entity.Migrations;

namespace DDrop.Db.Migrations
{
    public partial class PhotoOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Measurements", "PhotoOrderInSeries", c => c.Int(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Measurements", "PhotoOrderInSeries");
        }
    }
}