using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Contracts.Models
{
    public class RutaDto : BaseDto
    {
        public string Coordenadas { get; set; }
        public double? ZoomInicial { get; set; }
    }
}
