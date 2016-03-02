namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serverTrans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Server", "StageType", c => c.Int(nullable: false));
            DropColumn("dbo.Server", "EnvironmentType");
            DropColumn("dbo.Server", "SiteInfoJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Server", "EnvironmentType", c => c.Int(nullable: false));
            DropColumn("dbo.Server", "StageType");
            AddColumn("dbo.Server", "SiteInfoJson", c => c.String());
        }
    }
}
