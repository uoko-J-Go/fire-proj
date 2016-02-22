namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTaskEnvironment : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TaskInfo", "DeployEnvironment", c => c.Int(nullable: false));
            AlterColumn("dbo.TaskLogs", "Environment", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaskLogs", "Environment", c => c.String());
            AlterColumn("dbo.TaskInfo", "DeployEnvironment", c => c.String(nullable: false));
        }
    }
}
