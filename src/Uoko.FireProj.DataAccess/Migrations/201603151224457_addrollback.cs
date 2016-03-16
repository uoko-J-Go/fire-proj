namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addrollback : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RollbackTaskInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        ProjectName = c.String(),
                        FromVersion = c.String(maxLength: 50),
                        ToVersion = c.String(maxLength: 50),
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
            
            AddColumn("dbo.ProjectInfo", "OnlineVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectInfo", "OnlineVersion");
            DropTable("dbo.RollbackTaskInfo");
        }
    }
}
