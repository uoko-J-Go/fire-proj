namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateentity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dictionary",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nmae = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                        ParentId = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaskInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        TaskName = c.String(nullable: false, maxLength: 50),
                        Branch = c.String(nullable: false),
                        DeployEnvironment = c.String(nullable: false),
                        DeployIP = c.String(nullable: false),
                        SiteName = c.String(),
                        CheckUserId = c.String(),
                        NoticeUserId = c.String(),
                        DeployAddress = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        TaskDesc = c.String(),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaskLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        Branch = c.String(nullable: false),
                        DeployEnvironment = c.String(nullable: false),
                        DeployIP = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        OperationBy = c.String(nullable: false),
                        OperationDate = c.DateTime(nullable: false),
                        LogsText = c.String(),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Projec", "ProjectFileName", c => c.String(nullable: false));
            AddColumn("dbo.Projec", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.Projec", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Projec", "ModifyBy", c => c.Int());
            AddColumn("dbo.Projec", "ModifyDate", c => c.DateTime());
            DropColumn("dbo.Projec", "SiteNmae");
            DropColumn("dbo.Projec", "ServerIP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Projec", "ServerIP", c => c.String());
            AddColumn("dbo.Projec", "SiteNmae", c => c.String(nullable: false));
            DropColumn("dbo.Projec", "ModifyDate");
            DropColumn("dbo.Projec", "ModifyBy");
            DropColumn("dbo.Projec", "CreateDate");
            DropColumn("dbo.Projec", "CreateBy");
            DropColumn("dbo.Projec", "ProjectFileName");
            DropTable("dbo.TaskLogs");
            DropTable("dbo.TaskInfo");
            DropTable("dbo.ResourceInfo");
            DropTable("dbo.Dictionary");
        }
    }
}
