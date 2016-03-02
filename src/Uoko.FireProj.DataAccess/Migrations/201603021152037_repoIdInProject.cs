namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repoIdInProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectInfo", "RepoId", c => c.Int(nullable: false));
            DropColumn("dbo.ProjectInfo", "ProjectId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProjectInfo", "ProjectId", c => c.Int(nullable: false));
            DropColumn("dbo.ProjectInfo", "RepoId");
        }
    }
}
