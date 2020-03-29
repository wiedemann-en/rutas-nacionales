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
    class ProvinciaEntityMap : EntityTypeConfiguration<ProvinciaEntity>
    {
        public ProvinciaEntityMap()
        {
            this.ToTable("Provincia");

            this.HasKey(x => x.IdProvincia);

            this.Property(x => x.IdProvincia)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Nombre)
                .HasMaxLength(128)
                .IsRequired();

            this.Property(x => x.Key)
                .HasMaxLength(128)
                .IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, 
                    new IndexAnnotation(new IndexAttribute("IX_Provincia_Key") { IsUnique = true }));

            this.Property(x => x.Coordenadas)
                .HasMaxLength(4098)
                .IsOptional();

            this.Property(x => x.ZoomInicial)
                .IsOptional();
        }
    }
}
