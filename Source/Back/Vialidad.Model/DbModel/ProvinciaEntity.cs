using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Model.DbModel
{
    public class ProvinciaEntity
    {
        #region Constructors
        public ProvinciaEntity()
        {
        }
        #endregion

        #region Properties
        public int IdProvincia { get; set; }
        public string Nombre { get; set; }
        public string Key { get; set; }
        public string Coordenadas { get; set; }
        public double? ZoomInicial { get; set; }
        #endregion

        #region References
        public virtual ICollection<TramoEntity> Tramos { get; set; }
        #endregion
    }
}
