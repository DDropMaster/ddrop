namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MeasuringDevice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferencePhotos", "MeasuringDevice", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReferencePhotos", "MeasuringDevice");
        }
    }
}
