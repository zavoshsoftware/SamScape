namespace Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "ParentId", "dbo.Products");
            DropIndex("dbo.Products", new[] { "ParentId" });
            DropColumn("dbo.Products", "SecondImageUrl");
            DropColumn("dbo.Products", "PartnerAmount");
            DropColumn("dbo.Products", "ParentId");
            DropColumn("dbo.Products", "AgeRangeId");
            DropColumn("dbo.Products", "Size");
            DropColumn("dbo.Products", "Material");
            DropColumn("dbo.Products", "MoveAbility");
            DropColumn("dbo.Products", "WashAbility");
            DropColumn("dbo.Products", "PlaySoundAbility");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "PlaySoundAbility", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "WashAbility", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "MoveAbility", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "Material", c => c.String());
            AddColumn("dbo.Products", "Size", c => c.String());
            AddColumn("dbo.Products", "AgeRangeId", c => c.Guid());
            AddColumn("dbo.Products", "ParentId", c => c.Guid());
            AddColumn("dbo.Products", "PartnerAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "SecondImageUrl", c => c.String(maxLength: 500));
            CreateIndex("dbo.Products", "ParentId");
            AddForeignKey("dbo.Products", "ParentId", "dbo.Products", "Id");
        }
    }
}
