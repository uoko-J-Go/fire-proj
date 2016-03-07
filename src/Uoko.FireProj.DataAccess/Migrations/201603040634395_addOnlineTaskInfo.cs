namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOnlineTaskInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OnlineTaskInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OnlineVersion = c.String(nullable: false, maxLength: 50),
                        DeployIP = c.String(),
                        DeployAddress = c.String(),
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
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.OnlineTaskInfoes", "idx_OnlineVersion_unique");
            DropTable("dbo.OnlineTaskInfoes");
        }
    }
}
