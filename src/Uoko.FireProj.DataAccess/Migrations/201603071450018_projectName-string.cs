namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class projectNamestring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OnlineTaskInfoes", "ProjectName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OnlineTaskInfoes", "ProjectName", c => c.Int(nullable: false));
        }
    }
}
