namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProjectDomainRulefield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectInfo", "DomainRule", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectInfo", "DomainRule");
        }
    }
}
