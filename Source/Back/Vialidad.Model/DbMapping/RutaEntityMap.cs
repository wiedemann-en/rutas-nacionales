using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Model.DbModel;

namespace Vialidad.Model.DbMapping
{
    class RutaEntityMap : EntityTypeConfiguration<RutaEntity>
    {
        public RutaEntityMap()
        {
            this.ToTable("Ruta");

            this.HasKey(x => x.IdRuta);

            this.Property(x => x.IdRuta)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Nombre)
                .HasMaxLength(64)
                .IsRequired();

            this.Property(x => x.Key)
                .HasMaxLength(64)
                .IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Ruta_Key") { IsUnique = true }));

            this.Property(x => x.Coordenadas)
                .HasMaxLength(4098)
                .IsOptional();

            this.Property(x => x.ZoomInicial)
                .IsOptional();
        }
    }
}
