namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V05_Referencia_Fixs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Referencia", "Orden", c => c.Int());
            AddColumn("dbo.Referencia", "Ancho", c => c.Int());
            AddColumn("dbo.Referencia", "Alto", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Referencia", "Alto");
            DropColumn("dbo.Referencia", "Ancho");
            DropColumn("dbo.Referencia", "Orden");
        }
    }
}
