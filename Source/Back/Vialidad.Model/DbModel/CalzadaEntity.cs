using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Model.DbModel
{
    public class CalzadaEntity
    {
        #region Constructors
        public CalzadaEntity()
        {
        }
        #endregion

        #region Properties
        public int IdCalzada { get; set; }
        public string Nombre { get; set; }
        public string Key { get; set; }
        #endregion

        #region References
        public virtual ICollection<TramoEntity> Tramos { get; set; }
        #endregion
    }
}
