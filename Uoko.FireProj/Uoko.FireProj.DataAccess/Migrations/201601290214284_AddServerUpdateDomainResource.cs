namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServerUpdateDomainResource : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DomainResource",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        ServerId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
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
                        EnvironmentType = c.Int(nullable: false),
                        ServerDesc = c.String(),
                        Status = c.Int(nullable: false),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TaskLogs", "BuildId", c => c.Int(nullable: false));
            DropTable("dbo.ResourceInfo");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ResourceInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        DeployIPId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifyBy = c.Int(),
                        ModifyDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.TaskLogs", "BuildId");
            DropTable("dbo.Server");
            DropTable("dbo.DomainResource");
        }
    }
}
