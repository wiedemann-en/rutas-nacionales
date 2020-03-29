using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Model.DbModel
{
    public class TramoEntity
    {
        #region Constructors
        public TramoEntity()
        {
        }
        #endregion

        #region Properties
        public long IdTramo { get; set; }
        public int IdProvincia { get; set; }
        public int IdRuta { get; set; }
        public int IdCalzada { get; set; }
        public string TramoNormalizado { get; set; }
        public string TramoDesnormalizado { get; set; }
        public string Coordenadas { get; set; }
        public string Detalle { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public bool Activo { get; set; }
        public int? Orden { get; set; }
        public string JsonRouting { get; set; }
        #endregion

        #region References
        public virtual ProvinciaEntity Provincia { get; set; }
        public virtual RutaEntity Ruta { get; set; }
        public virtual CalzadaEntity Calzada { get; set; }
        #endregion
    }
}
