namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTask11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DomainResource", "ServerIP", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DomainResource", "ServerIP", c => c.String());
        }
    }
}
