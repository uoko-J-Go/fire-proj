namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDomainResourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DomainResource", "ServerId", c => c.Int(nullable: false));
            DropColumn("dbo.DomainResource", "ServerIP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DomainResource", "ServerIP", c => c.Int(nullable: false));
            DropColumn("dbo.DomainResource", "ServerId");
        }
    }
}
