namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNameForBaseEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dictionary", "CreatorId", c => c.Int(nullable: false));
            AddColumn("dbo.Dictionary", "CreatorName", c => c.String());
            AddColumn("dbo.Dictionary", "ModifyId", c => c.Int());
            AddColumn("dbo.Dictionary", "ModifierName", c => c.String());
            AddColumn("dbo.DomainResource", "CreatorId", c => c.Int(nullable: false));
            AddColumn("dbo.DomainResource", "CreatorName", c => c.String());
            AddColumn("dbo.DomainResource", "ModifyId", c => c.Int());
            AddColumn("dbo.DomainResource", "ModifierName", c => c.String());
            AddColumn("dbo.ProjectInfo", "CreatorId", c => c.Int(nullable: false));
            AddColumn("dbo.ProjectInfo", "CreatorName", c => c.String());
            AddColumn("dbo.ProjectInfo", "ModifyId", c => c.Int());
            AddColumn("dbo.ProjectInfo", "ModifierName", c => c.String());
            AddColumn("dbo.Server", "CreatorId", c => c.Int(nullable: false));
            AddColumn("dbo.Server", "CreatorName", c => c.String());
            AddColumn("dbo.Server", "ModifyId", c => c.Int());
            AddColumn("dbo.Server", "ModifierName", c => c.String());
            AddColumn("dbo.TaskInfo", "CreatorId", c => c.Int(nullable: false));
            AddColumn("dbo.TaskInfo", "CreatorName", c => c.String());
            AddColumn("dbo.TaskInfo", "ModifyId", c => c.Int());
            AddColumn("dbo.TaskInfo", "ModifierName", c => c.String());
            AddColumn("dbo.TaskLogs", "CreatorId", c => c.Int(nullable: false));
            AddColumn("dbo.TaskLogs", "CreatorName", c => c.String());
            AddColumn("dbo.TaskLogs", "ModifyId", c => c.Int());
            AddColumn("dbo.TaskLogs", "ModifierName", c => c.String());
            DropColumn("dbo.Dictionary", "CreateBy");
            DropColumn("dbo.Dictionary", "ModifyBy");
            DropColumn("dbo.DomainResource", "CreateBy");
            DropColumn("dbo.DomainResource", "ModifyBy");
            DropColumn("dbo.ProjectInfo", "CreateBy");
            DropColumn("dbo.ProjectInfo", "ModifyBy");
            DropColumn("dbo.Server", "CreateBy");
            DropColumn("dbo.Server", "ModifyBy");
            DropColumn("dbo.TaskInfo", "CreateBy");
            DropColumn("dbo.TaskInfo", "ModifyBy");
            DropColumn("dbo.TaskLogs", "CreateBy");
            DropColumn("dbo.TaskLogs", "ModifyBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaskLogs", "ModifyBy", c => c.Int());
            AddColumn("dbo.TaskLogs", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.TaskInfo", "ModifyBy", c => c.Int());
            AddColumn("dbo.TaskInfo", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.Server", "ModifyBy", c => c.Int());
            AddColumn("dbo.Server", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.ProjectInfo", "ModifyBy", c => c.Int());
            AddColumn("dbo.ProjectInfo", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.DomainResource", "ModifyBy", c => c.Int());
            AddColumn("dbo.DomainResource", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.Dictionary", "ModifyBy", c => c.Int());
            AddColumn("dbo.Dictionary", "CreateBy", c => c.Int(nullable: false));
            DropColumn("dbo.TaskLogs", "ModifierName");
            DropColumn("dbo.TaskLogs", "ModifyId");
            DropColumn("dbo.TaskLogs", "CreatorName");
            DropColumn("dbo.TaskLogs", "CreatorId");
            DropColumn("dbo.TaskInfo", "ModifierName");
            DropColumn("dbo.TaskInfo", "ModifyId");
            DropColumn("dbo.TaskInfo", "CreatorName");
            DropColumn("dbo.TaskInfo", "CreatorId");
            DropColumn("dbo.Server", "ModifierName");
            DropColumn("dbo.Server", "ModifyId");
            DropColumn("dbo.Server", "CreatorName");
            DropColumn("dbo.Server", "CreatorId");
            DropColumn("dbo.ProjectInfo", "ModifierName");
            DropColumn("dbo.ProjectInfo", "ModifyId");
            DropColumn("dbo.ProjectInfo", "CreatorName");
            DropColumn("dbo.ProjectInfo", "CreatorId");
            DropColumn("dbo.DomainResource", "ModifierName");
            DropColumn("dbo.DomainResource", "ModifyId");
            DropColumn("dbo.DomainResource", "CreatorName");
            DropColumn("dbo.DomainResource", "CreatorId");
            DropColumn("dbo.Dictionary", "ModifierName");
            DropColumn("dbo.Dictionary", "ModifyId");
            DropColumn("dbo.Dictionary", "CreatorName");
            DropColumn("dbo.Dictionary", "CreatorId");
        }
    }
}
