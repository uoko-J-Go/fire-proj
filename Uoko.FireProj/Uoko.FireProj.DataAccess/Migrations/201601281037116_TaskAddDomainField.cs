namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TaskAddDomainField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskInfo", "Domain", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskInfo", "Domain");
        }
    }
}
