namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DomainResource", "DeployIP", c => c.String());
            DropColumn("dbo.DomainResource", "ServerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DomainResource", "ServerId", c => c.Int(nullable: false));
            DropColumn("dbo.DomainResource", "DeployIP");
        }
    }
}
