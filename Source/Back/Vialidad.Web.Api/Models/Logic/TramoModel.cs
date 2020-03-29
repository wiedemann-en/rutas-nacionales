using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vialidad.Web.Api.Models.Logic
{
    public class TramoModel
    {
        public TramoModel()
        {
            Coordenadas = new List<CoordenadaModel>();
            Referencias = new List<ReferenciaModel>();
        }

        public long IdTramo { get; set; }
        public long IdProvincia { get; set; }
        public string Provincia { get; set; }
        public long IdRuta { get; set; }
        public string Ruta { get; set; }
        public long IdCalzada { get; set; }
        public string Calzada { get; set; }
        public string Tramo { get; set; }
        public string Detalle { get; set; }
        public string Observaciones { get; set; }
        public string FechaActualizacion { get; set; }
        public int? Orden { get; set; }
        public List<CoordenadaModel> Coordenadas { get; set; }
        public List<ReferenciaModel> Referencias { get; set; }
        public string ColorRuta { get; set; }
        public string ColorFechaActualizacion { get; set; }
    }
}