using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Model.DbModel;

namespace Vialidad.Model.DbMapping
{
    class TramoEntityMap : EntityTypeConfiguration<TramoEntity>
    {
        public TramoEntityMap()
        {
            this.ToTable("Tramo");

            this.HasKey(x => x.IdTramo);

            this.Property(x => x.IdTramo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.IdProvincia)
                .IsRequired();

            this.Property(x => x.IdRuta)
                .IsRequired();

            this.Property(x => x.IdCalzada)
                .IsRequired();

            this.Property(x => x.TramoNormalizado)
                .HasMaxLength(512)
                .IsRequired();

            this.Property(x => x.TramoDesnormalizado)
                .HasMaxLength(512)
                .IsOptional();

            this.Property(x => x.Coordenadas)
                .HasMaxLength(4098)
                .IsOptional();

            this.Property(x => x.Detalle)
                .IsMaxLength()
                .IsOptional();

            this.Property(x => x.Observaciones)
                .IsMaxLength()
                .IsOptional();

            this.Property(x => x.FechaAlta)
                .HasColumnType("datetime2")
                .IsRequired();

            this.Property(x => x.FechaActualizacion)
                .HasColumnType("datetime2")
                .IsOptional();

            this.Property(x => x.Activo)
                .IsRequired();

            this.Property(x => x.Orden)
                .IsOptional();

            this.Property(x => x.JsonRouting)
                .IsMaxLength()
                .IsOptional();

            //References
            this.HasRequired(s => s.Provincia)
                .WithMany(c => c.Tramos)
                .HasForeignKey(s => s.IdProvincia);

            this.HasRequired(s => s.Ruta)
                .WithMany(c => c.Tramos)
                .HasForeignKey(s => s.IdRuta);

            this.HasRequired(s => s.Calzada)
                .WithMany(c => c.Tramos)
                .HasForeignKey(s => s.IdCalzada);
        }
    }
}
