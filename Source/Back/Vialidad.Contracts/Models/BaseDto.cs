using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Contracts.Models
{
    public class BaseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Key { get; set; }
    }
}
