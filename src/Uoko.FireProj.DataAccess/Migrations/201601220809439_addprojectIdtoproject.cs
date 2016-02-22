namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addprojectIdtoproject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectInfo", "ProjectId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectInfo", "ProjectId");
        }
    }
}
