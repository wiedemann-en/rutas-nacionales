using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Model.DbModel;

namespace Vialidad.Model
{
    public class VialidadContext : DbContext
    {
        #region Constructores y privados
        public VialidadContext()
            : base("DbVialidadRutas")
        {
        }
        #endregion

        #region DbSets
        public DbSet<CalzadaEntity> CalzadaDataSet { get; set; }
        public DbSet<LogEntity> LogDataSet { get; set; }
        public DbSet<ProvinciaEntity> ProvinciaDataSet { get; set; }
        public DbSet<ReferenciaEntity> ReferenciaDataSet { get; set; }
        public DbSet<RutaEntity> RutaDataSet { get; set; }
        public DbSet<TramoEntity> TramoDataSet { get; set; }
        #endregion

        #region Model Creating
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new DbMapping.CalzadaEntityMap());
            modelBuilder.Configurations.Add(new DbMapping.LogEntityMap());
            modelBuilder.Configurations.Add(new DbMapping.ProvinciaEntityMap());
            modelBuilder.Configurations.Add(new DbMapping.ReferenciaEntityMap());
            modelBuilder.Configurations.Add(new DbMapping.RutaEntityMap());
            modelBuilder.Configurations.Add(new DbMapping.TramoEntityMap());
        }
        #endregion
    }
}
