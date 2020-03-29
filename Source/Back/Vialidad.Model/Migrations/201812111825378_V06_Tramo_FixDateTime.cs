namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V06_Tramo_FixDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tramo", "FechaAlta", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Tramo", "FechaActualizacion", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tramo", "FechaActualizacion", c => c.DateTime());
            AlterColumn("dbo.Tramo", "FechaAlta", c => c.DateTime(nullable: false));
        }
    }
}
