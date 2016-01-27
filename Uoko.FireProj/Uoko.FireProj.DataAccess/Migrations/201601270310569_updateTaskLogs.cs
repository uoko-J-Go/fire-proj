namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTaskLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskLogs", "TriggeredId", c => c.Int(nullable: false));
            AddColumn("dbo.TaskLogs", "LogsDesc", c => c.String());
            AddColumn("dbo.TaskLogs", "TaskLogsType", c => c.Int(nullable: false));
            DropColumn("dbo.TaskLogs", "Branch");
            DropColumn("dbo.TaskLogs", "DeployEnvironment");
            DropColumn("dbo.TaskLogs", "DeployIP");
            DropColumn("dbo.TaskLogs", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaskLogs", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.TaskLogs", "DeployIP", c => c.String(nullable: false));
            AddColumn("dbo.TaskLogs", "DeployEnvironment", c => c.String(nullable: false));
            AddColumn("dbo.TaskLogs", "Branch", c => c.String(nullable: false));
            DropColumn("dbo.TaskLogs", "TaskLogsType");
            DropColumn("dbo.TaskLogs", "LogsDesc");
            DropColumn("dbo.TaskLogs", "TriggeredId");
        }
    }
}
