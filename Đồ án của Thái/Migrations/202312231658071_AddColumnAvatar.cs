﻿namespace Đồ_án_của_Thái.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnAvatar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Avatar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Avatar");
        }
    }
}
