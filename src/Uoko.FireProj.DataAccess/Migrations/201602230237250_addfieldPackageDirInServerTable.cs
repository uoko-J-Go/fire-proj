namespace Uoko.FireProj.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfieldPackageDirInServerTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Server", "PackageDir", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Server", "PackageDir");
        }
    }
}
