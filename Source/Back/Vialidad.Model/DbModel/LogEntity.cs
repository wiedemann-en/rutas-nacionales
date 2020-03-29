using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Model.DbModel
{
    public class LogEntity
    {
        public long IdLog { get; set; }
        public string TipoLog { get; set; }
        public string Origen { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string UtcOffSet { get; set; }
        public string Detalle { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string TargetSite { get; set; }
    }
}
