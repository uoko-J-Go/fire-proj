namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedictionary : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dictionary", "ParentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Dictionary", "ParentId", c => c.String());
        }
    }
}
