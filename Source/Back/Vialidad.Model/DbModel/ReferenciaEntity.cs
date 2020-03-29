using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Model.DbModel
{
    public class ReferenciaEntity
    {
        #region Constructors
        public ReferenciaEntity()
        {
        }
        #endregion

        #region Properties
        public int IdReferencia { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Imagen { get; set; }
        public string PalabrasClaves { get; set; }
        public int? Orden { get; set; }
        public int? Ancho { get; set; }
        public int? Alto { get; set; }
        #endregion
    }
}
