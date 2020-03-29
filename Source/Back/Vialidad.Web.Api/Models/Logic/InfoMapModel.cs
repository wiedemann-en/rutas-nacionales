using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vialidad.Web.Api.Models.Logic
{
    public class InfoMapModel
    {
        public InfoMapModel()
        {
            Coordenadas = new CoordenadaModel();
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Zoom { get; set; }
        public CoordenadaModel Coordenadas { get; set; }
    }
}