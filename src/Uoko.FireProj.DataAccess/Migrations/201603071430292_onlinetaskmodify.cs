namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onlinetaskmodify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineTaskInfoes", "DeployServerId", c => c.Int(nullable: false));
            AddColumn("dbo.OnlineTaskInfoes", "DeployServerIP", c => c.String());
            DropColumn("dbo.OnlineTaskInfoes", "DeployIP");
            DropColumn("dbo.OnlineTaskInfoes", "DeployAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OnlineTaskInfoes", "DeployAddress", c => c.String());
            AddColumn("dbo.OnlineTaskInfoes", "DeployIP", c => c.String());
            DropColumn("dbo.OnlineTaskInfoes", "DeployServerIP");
            DropColumn("dbo.OnlineTaskInfoes", "DeployServerId");
        }
    }
}
