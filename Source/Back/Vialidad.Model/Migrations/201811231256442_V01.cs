namespace Vialidad.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Calzada",
                c => new
                    {
                        IdCalzada = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 64),
                        Key = c.String(nullable: false, maxLength: 64),
                    })
                .PrimaryKey(t => t.IdCalzada)
                .Index(t => t.Key, unique: true, name: "IX_Calzada_Key");
            
            CreateTable(
                "dbo.Tramo",
                c => new
                    {
                        IdTramo = c.Long(nullable: false, identity: true),
                        IdProvincia = c.Int(nullable: false),
                        IdRuta = c.Int(nullable: false),
                        IdCalzada = c.Int(nullable: false),
                        TramoNormalizado = c.String(nullable: false, maxLength: 512),
                        TramoDesnormalizado = c.String(maxLength: 512),
                        Coordenadas = c.String(),
                        Detalle = c.String(),
                        Observaciones = c.String(),
                        FechaAlta = c.DateTime(nullable: false),
                        FechaActualizacion = c.DateTime(),
                        Activo = c.Boolean(nullable: false),
                        Orden = c.Int(),
                    })
                .PrimaryKey(t => t.IdTramo)
                .ForeignKey("dbo.Calzada", t => t.IdCalzada, cascadeDelete: true)
                .ForeignKey("dbo.Provincia", t => t.IdProvincia, cascadeDelete: true)
                .ForeignKey("dbo.Ruta", t => t.IdRuta, cascadeDelete: true)
                .Index(t => t.IdProvincia)
                .Index(t => t.IdRuta)
                .Index(t => t.IdCalzada);
            
            CreateTable(
                "dbo.Provincia",
                c => new
                    {
                        IdProvincia = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 128),
                        Key = c.String(nullable: false, maxLength: 128),
                        Coordenadas = c.String(),
                        ZoomInicial = c.Int(),
                    })
                .PrimaryKey(t => t.IdProvincia)
                .Index(t => t.Key, unique: true, name: "IX_Provincia_Key");
            
            CreateTable(
                "dbo.Ruta",
                c => new
                    {
                        IdRuta = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 64),
                        Key = c.String(nullable: false, maxLength: 64),
                        Coordenadas = c.String(),
                        ZoomInicial = c.Int(),
                    })
                .PrimaryKey(t => t.IdRuta)
                .Index(t => t.Key, unique: true, name: "IX_Ruta_Key");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tramo", "IdRuta", "dbo.Ruta");
            DropForeignKey("dbo.Tramo", "IdProvincia", "dbo.Provincia");
            DropForeignKey("dbo.Tramo", "IdCalzada", "dbo.Calzada");
            DropIndex("dbo.Ruta", "IX_Ruta_Key");
            DropIndex("dbo.Provincia", "IX_Provincia_Key");
            DropIndex("dbo.Tramo", new[] { "IdCalzada" });
            DropIndex("dbo.Tramo", new[] { "IdRuta" });
            DropIndex("dbo.Tramo", new[] { "IdProvincia" });
            DropIndex("dbo.Calzada", "IX_Calzada_Key");
            DropTable("dbo.Ruta");
            DropTable("dbo.Provincia");
            DropTable("dbo.Tramo");
            DropTable("dbo.Calzada");
        }
    }
}
