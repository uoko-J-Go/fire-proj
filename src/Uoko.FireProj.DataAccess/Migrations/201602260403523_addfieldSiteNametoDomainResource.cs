namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfieldSiteNametoDomainResource : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DomainResource", "SiteName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DomainResource", "SiteName");
        }
    }
}
