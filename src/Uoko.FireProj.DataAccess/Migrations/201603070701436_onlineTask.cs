namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlineTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineTaskInfoes", "DeployServerName", c => c.String());
            DropColumn("dbo.TaskInfo", "HasOnlineDeployed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaskInfo", "HasOnlineDeployed", c => c.Boolean(nullable: false));
            DropColumn("dbo.OnlineTaskInfoes", "DeployServerName");
        }
    }
}
