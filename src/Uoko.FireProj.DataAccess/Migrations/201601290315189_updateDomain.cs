namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDomain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DomainResource", "ServerIP", c => c.String());
            DropColumn("dbo.DomainResource", "DeployIP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DomainResource", "DeployIP", c => c.String());
            DropColumn("dbo.DomainResource", "ServerIP");
        }
    }
}
