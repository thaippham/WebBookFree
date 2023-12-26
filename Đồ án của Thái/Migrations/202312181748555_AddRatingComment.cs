namespace Đồ_án_của_Thái.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRatingComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "rating", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "rating");
        }
    }
}
