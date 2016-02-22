namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProjectfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectInfo", "ProjectSlnName", c => c.String(nullable: false));
            AddColumn("dbo.ProjectInfo", "ProjectCsprojName", c => c.String());
            DropColumn("dbo.ProjectInfo", "ProjectFileName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectInfo", "ProjectFileName", c => c.String(nullable: false));
            DropColumn("dbo.ProjectInfo", "ProjectCsprojName");
            DropColumn("dbo.ProjectInfo", "ProjectSlnName");
        }
    }
}
