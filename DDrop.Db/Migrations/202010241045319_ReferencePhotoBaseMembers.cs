namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferencePhotoBaseMembers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferencePhotos", "AddedDate", c => c.String());
            AddColumn("dbo.ReferencePhotos", "CreationDateTime", c => c.String());
            AddColumn("dbo.ReferencePhotos", "PhotoType", c => c.String());
            Sql(@"UPDATE dbo.ReferencePhotos SET PhotoType = 'ReferencePhoto'");
            Sql(@"UPDATE dbo.ReferencePhotos SET AddedDate = '09/08/2020 14:58:14'");
            Sql(@"UPDATE dbo.ReferencePhotos SET CreationDateTime = '09/08/2020 14:58:14'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReferencePhotos", "PhotoType");
            DropColumn("dbo.ReferencePhotos", "CreationDateTime");
            DropColumn("dbo.ReferencePhotos", "AddedDate");
        }
    }
}
