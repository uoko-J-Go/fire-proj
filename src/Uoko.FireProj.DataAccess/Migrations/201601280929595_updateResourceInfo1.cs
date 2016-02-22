namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateResourceInfo1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceInfo", "DeployIPId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResourceInfo", "DeployIPId");
        }
    }
}
