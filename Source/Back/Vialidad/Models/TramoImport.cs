using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Models
{
    public class TramoImport
    {
        public string Provincia { get; set; }
        public string Ruta { get; set; }
        public string TramoNormalizado { get; set; }
        public string TramoDesnormalizado { get; set; }
        public string Calzada { get; set; }
        public string Detalle { get; set; }
        public string Observaciones { get; set; }
        public DateTime Actualizacion { get; set; }
        public string Coordenadas { get; set; }

        #region Overrides
        public override string ToString()
        {
            return string.Format("{0}@{1}@{2}",
                Provincia.ToUpper().Replace(" ", string.Empty),
                Ruta.ToUpper().Replace(" ", string.Empty),
                TramoNormalizado.ToUpper().Replace(" ", string.Empty));
        }
        #endregion
    }
}
