namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Provincia", "ZoomInicial", c => c.Double());
            AlterColumn("dbo.Ruta", "ZoomInicial", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ruta", "ZoomInicial", c => c.Int());
            AlterColumn("dbo.Provincia", "ZoomInicial", c => c.Int());
        }
    }
}
