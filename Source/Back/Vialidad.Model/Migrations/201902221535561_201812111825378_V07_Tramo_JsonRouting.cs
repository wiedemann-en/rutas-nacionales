namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201812111825378_V07_Tramo_JsonRouting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tramo", "JsonRouting", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tramo", "JsonRouting");
        }
    }
}
