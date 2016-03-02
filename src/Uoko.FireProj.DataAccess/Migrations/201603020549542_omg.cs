namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class omg : DbMigration
    {
        public override void Up()
        {
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
                HasOnlineDeployed = c.Boolean(nullable: false),
                OnlineTaskId = c.Int(),
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
                    DeployInfo = c.String(),
                    Stage = c.Int(nullable: false),
                    Comments = c.String(),
                    LogType = c.Int(nullable: false),
                    CreateBy = c.Int(nullable: false),
                    CreateDate = c.DateTime(nullable: false),
                    ModifyBy = c.Int(),
                    ModifyDate = c.DateTime(),
                })
                .PrimaryKey(t => t.Id);



        }
        
        public override void Down()
        {
            DropTable("dbo.TaskLogs");
            DropTable("dbo.TaskInfo");
        }
    }
}
