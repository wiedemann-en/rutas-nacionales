using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vialidad.Web.Api.Models.Logic
{
    public class CoordenadaModel
    {
        public CoordenadaModel()
        {
        }
        public CoordenadaModel(string latitud, string longitud)
        {
            Latitud = latitud;
            Longitud = longitud;
        }

        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}