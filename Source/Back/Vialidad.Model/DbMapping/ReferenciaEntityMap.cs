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
    class ReferenciaEntityMap : EntityTypeConfiguration<ReferenciaEntity>
    {
        public ReferenciaEntityMap()
        {
            this.ToTable("Referencia");

            this.HasKey(x => x.IdReferencia);

            this.Property(x => x.IdReferencia)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Nombre)
                .HasMaxLength(128)
                .IsRequired();

            this.Property(x => x.Tipo)
                .HasMaxLength(32)
                .IsRequired();

            this.Property(x => x.Imagen)
                .HasMaxLength(128)
                .IsOptional();

            this.Property(x => x.PalabrasClaves)
                .IsMaxLength()
                .IsOptional();

            this.Property(x => x.Orden)
                .IsOptional();

            this.Property(x => x.Ancho)
                .IsOptional();

            this.Property(x => x.Alto)
                .IsOptional();
        }
    }
}
