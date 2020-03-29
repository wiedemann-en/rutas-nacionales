using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Models;

namespace Vialidad.Download
{
    public class ItemRutaComparer : IEqualityComparer<TramoImport>
    {
        public bool Equals(TramoImport x, TramoImport y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(TramoImport obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
