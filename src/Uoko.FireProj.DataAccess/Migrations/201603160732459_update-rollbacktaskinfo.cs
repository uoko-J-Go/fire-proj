namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updaterollbacktaskinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RollbackTaskInfo", "DeployServerId", c => c.Int(nullable: false));
            AddColumn("dbo.RollbackTaskInfo", "DeployServerIP", c => c.String());
            AddColumn("dbo.RollbackTaskInfo", "DeployServerName", c => c.String());
            AddColumn("dbo.RollbackTaskInfo", "Domain", c => c.String());
            AddColumn("dbo.RollbackTaskInfo", "SiteName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RollbackTaskInfo", "SiteName");
            DropColumn("dbo.RollbackTaskInfo", "Domain");
            DropColumn("dbo.RollbackTaskInfo", "DeployServerName");
            DropColumn("dbo.RollbackTaskInfo", "DeployServerIP");
            DropColumn("dbo.RollbackTaskInfo", "DeployServerId");
        }
    }
}
