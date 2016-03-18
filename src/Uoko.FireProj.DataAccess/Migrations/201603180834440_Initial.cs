namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dictionary",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                        ParentId = c.Int(nullable: false),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DomainResource",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServerId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        SiteName = c.String(),
                        ProjectId = c.Int(nullable: false),
                        TaskId = c.Int(),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OnlineTaskInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OnlineVersion = c.String(nullable: false, maxLength: 50),
                        ProjectId = c.Int(nullable: false),
                        ProjectName = c.String(),
                        DeployServerId = c.Int(nullable: false),
                        DeployServerIP = c.String(),
                        DeployServerName = c.String(),
                        Domain = c.String(),
                        SiteName = c.String(),
                        DeployStatus = c.Int(nullable: false),
                        TriggeredId = c.Int(),
                        BuildId = c.Int(),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.OnlineVersion, unique: true, name: "idx_OnlineVersion_unique");
            
            CreateTable(
                "dbo.ProjectInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 50),
                        ProjectRepo = c.String(nullable: false),
                        RepoId = c.Int(nullable: false),
                        ProjectSlnName = c.String(nullable: false),
                        ProjectCsprojName = c.String(),
                        ProjectDesc = c.String(),
                        DomainRule = c.String(),
                        OnlineVersion = c.String(),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RollbackTaskInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        ProjectName = c.String(),
                        FromVersion = c.String(maxLength: 50),
                        ToVersion = c.String(maxLength: 50),
                        DeployServerId = c.Int(nullable: false),
                        DeployServerIP = c.String(),
                        DeployServerName = c.String(),
                        Domain = c.String(),
                        SiteName = c.String(),
                        DeployStatus = c.Int(nullable: false),
                        TriggeredId = c.Int(),
                        BuildId = c.Int(),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Server",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IP = c.String(nullable: false),
                        StageType = c.Int(nullable: false),
                        ServerDesc = c.String(),
                        PackageDir = c.String(),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaskInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskName = c.String(nullable: false, maxLength: 50),
                        ProjectId = c.Int(nullable: false),
                        Branch = c.String(nullable: false),
                        DeployInfoIocJson = c.String(),
                        IocCheckUserId = c.String(),
                        DeployInfoPreJson = c.String(),
                        PreCheckUserId = c.String(),
                        DeployInfoOnlineJson = c.String(),
                        OnlineCheckUserId = c.String(),
                        OnlineTaskId = c.Int(),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.TaskName, unique: true, name: "idx_taskName_unique");
            
            CreateTable(
                "dbo.TaskLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        DeployInfo = c.String(),
                        Stage = c.Int(nullable: false),
                        Comments = c.String(),
                        TriggeredId = c.Int(),
                        LogType = c.Int(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        CreatorName = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyId = c.Int(),
                        ModifierName = c.String(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TaskInfo", "idx_taskName_unique");
            DropIndex("dbo.OnlineTaskInfoes", "idx_OnlineVersion_unique");
            DropTable("dbo.TaskLogs");
            DropTable("dbo.TaskInfo");
            DropTable("dbo.Server");
            DropTable("dbo.RollbackTaskInfo");
            DropTable("dbo.ProjectInfo");
            DropTable("dbo.OnlineTaskInfoes");
            DropTable("dbo.DomainResource");
            DropTable("dbo.Dictionary");
        }
    }
}
