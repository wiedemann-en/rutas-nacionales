namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V03_Referencia : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Referencia",
                c => new
                    {
                        IdReferencia = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 128),
                        Tipo = c.String(nullable: false, maxLength: 32),
                        Imagen = c.String(maxLength: 128),
                        PalabrasClaves = c.String(),
                    })
                .PrimaryKey(t => t.IdReferencia);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Referencia");
        }
    }
}
