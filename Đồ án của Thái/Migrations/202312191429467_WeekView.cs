namespace Đồ_án_của_Thái.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeekView : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comics", "WeekView", c => c.Int(nullable: false));
            AddColumn("dbo.Comics", "DateWeekView", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comics", "DateWeekView");
            DropColumn("dbo.Comics", "WeekView");
        }
    }
}
