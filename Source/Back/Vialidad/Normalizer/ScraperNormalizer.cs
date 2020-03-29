using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Services;
using Vialidad.Services.Normalizer;

namespace Vialidad.Normalizer
{
    public class ScraperNormalizer
    {
        public void NormalizeDb()
        {
            IServiceTramo serviceTramo = new ServiceTramo();
            IEnumerable<TramoDto> tramos = serviceTramo.GetAll();
            foreach (var itemTramo in tramos)
            {
                itemTramo.TramoNormalizado = NormalizerTramo.Normalize(itemTramo.TramoNormalizado);
                serviceTramo.CreateOrUpdate(itemTramo);
            }
        }
    }
}
