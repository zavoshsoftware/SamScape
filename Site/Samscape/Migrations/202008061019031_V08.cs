namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V08 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sliders", "Title2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sliders", "Title2");
        }
    }
}
