namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Projec", newName: "ProjectInfo");
            AddColumn("dbo.Dictionary", "Name", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Dictionary", "Nmae");
            DropColumn("dbo.TaskLogs", "OperationBy");
            DropColumn("dbo.TaskLogs", "OperationDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaskLogs", "OperationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.TaskLogs", "OperationBy", c => c.String(nullable: false));
            AddColumn("dbo.Dictionary", "Nmae", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Dictionary", "Name");
            RenameTable(name: "dbo.ProjectInfo", newName: "Projec");
        }
    }
}
