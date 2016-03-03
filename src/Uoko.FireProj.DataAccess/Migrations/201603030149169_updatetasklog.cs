namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetasklog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskLogs", "TriggeredId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskLogs", "TriggeredId");
        }
    }
}
