namespace ActivityLog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ActivityModels", "UserId");
            AddForeignKey("dbo.ActivityModels", "UserId", "dbo.UserModels", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityModels", "UserId", "dbo.UserModels");
            DropIndex("dbo.ActivityModels", new[] { "UserId" });
        }
    }
}