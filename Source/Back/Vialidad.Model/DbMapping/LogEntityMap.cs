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
    class LogEntityMap : EntityTypeConfiguration<LogEntity>
    {
        public LogEntityMap()
        {
            this.ToTable("Log");

            this.HasKey(x => x.IdLog);

            this.Property(x => x.IdLog)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(l => l.TipoLog)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(l => l.Origen)
                .IsMaxLength()
                .IsRequired();

            this.Property(l => l.Descripcion)
                .IsMaxLength()
                .IsRequired();

            this.Property(l => l.Fecha)
                .IsRequired();

            this.Property(l => l.Detalle)
                .IsOptional()
                .IsMaxLength()
                .IsUnicode();

            this.Property(l => l.StackTrace)
                .IsOptional()
                .IsMaxLength()
                .IsUnicode();
        }
    }
}
