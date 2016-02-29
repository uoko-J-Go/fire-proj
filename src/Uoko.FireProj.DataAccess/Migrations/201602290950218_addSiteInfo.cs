namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSiteInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Server", "SiteInfoJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Server", "SiteInfoJson");
        }
    }
}
