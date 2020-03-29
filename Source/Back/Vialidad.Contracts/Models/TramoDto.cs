using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Contracts.Models
{
    public class TramoDto
    {
        public long IdTramo { get; set; }
        public int IdProvincia { get; set; }
        public int IdRuta { get; set; }
        public int IdCalzada { get; set; }
        public string TramoNormalizado { get; set; }
        public string TramoDesnormalizado { get; set; }
        public string Detalle { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Coordenadas { get; set; }
        public int? Orden { get; set; }
        public string JsonRouting { get; set; }
    }
}
