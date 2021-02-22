namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ROI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "RegionOfInterest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Series", "RegionOfInterest");
        }
    }
}
