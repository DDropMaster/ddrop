namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EllipseCoordinate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ThermalPhotos", "EllipseCoordinate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ThermalPhotos", "EllipseCoordinate");
        }
    }
}
