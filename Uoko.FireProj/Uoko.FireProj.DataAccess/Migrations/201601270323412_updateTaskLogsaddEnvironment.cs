namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTaskLogsaddEnvironment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskLogs", "Environment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskLogs", "Environment");
        }
    }
}
