namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addproject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projec",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 50),
                        SiteNmae = c.String(nullable: false),
                        ProjectRepo = c.String(nullable: false),
                        ProjectDesc = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Projec");
        }
    }
}
