namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V03_Log : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Log",
                c => new
                    {
                        IdLog = c.Long(nullable: false, identity: true),
                        TipoLog = c.String(nullable: false, maxLength: 1),
                        Origen = c.String(nullable: false),
                        Descripcion = c.String(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                        UtcOffSet = c.String(),
                        Detalle = c.String(),
                        StackTrace = c.String(),
                        Source = c.String(),
                        TargetSite = c.String(),
                    })
                .PrimaryKey(t => t.IdLog);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Log");
        }
    }
}
