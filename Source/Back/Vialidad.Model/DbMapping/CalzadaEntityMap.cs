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
    class CalzadaEntityMap : EntityTypeConfiguration<CalzadaEntity>
    {
        public CalzadaEntityMap()
        {
            this.ToTable("Calzada");

            this.HasKey(x => x.IdCalzada);

            this.Property(x => x.IdCalzada)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Nombre)
                .HasMaxLength(64)
                .IsRequired();

            this.Property(x => x.Key)
                .HasMaxLength(64)
                .IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_Calzada_Key") { IsUnique = true }));
        }
    }
}
