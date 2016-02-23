namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateResourceInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceInfo", "ProjectId", c => c.Int(nullable: false));
            AlterColumn("dbo.ResourceInfo", "TaskId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResourceInfo", "TaskId", c => c.String());
            DropColumn("dbo.ResourceInfo", "ProjectId");
        }
    }
}
