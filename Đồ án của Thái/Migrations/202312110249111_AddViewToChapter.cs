namespace Đồ_án_của_Thái.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddViewToChapter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chapters", "View", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chapters", "View");
        }
    }
}
