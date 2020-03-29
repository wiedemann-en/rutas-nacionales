using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Contracts.Models
{
    public class ReferenciaDto
    {
        public int IdReferencia { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Imagen { get; set; }
        public string PalabrasClaves { get; set; }
        public int Orden { get; set; }
        public int Ancho { get; set; }
        public int Alto { get; set; }
    }
}
