namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateprojectserverip : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projec", "ServerIP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projec", "ServerIP");
        }
    }
}
