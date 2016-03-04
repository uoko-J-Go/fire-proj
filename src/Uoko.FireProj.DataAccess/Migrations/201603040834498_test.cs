namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlineTaskInfoes", "ProjectId", c => c.Int(nullable: false));
            AddColumn("dbo.OnlineTaskInfoes", "ProjectName", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OnlineTaskInfoes", "ProjectName");
            DropColumn("dbo.OnlineTaskInfoes", "ProjectId");
        }
    }
}
