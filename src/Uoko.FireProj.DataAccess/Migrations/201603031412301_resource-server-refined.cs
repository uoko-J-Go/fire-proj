namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourceserverrefined : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DomainResource", "TaskId", c => c.Int());
            DropColumn("dbo.DomainResource", "Status");
            DropColumn("dbo.Server", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Server", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.DomainResource", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.DomainResource", "TaskId", c => c.Int(nullable: false));
        }
    }
}
