namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addremark : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskLogs", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskLogs", "Remark");
        }
    }
}
