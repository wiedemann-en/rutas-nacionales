using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;

namespace Vialidad.Contracts.Services
{
    public interface IServiceTramo
    {
        IEnumerable<TramoDto> GetAll(bool onlyActives = true);
        IEnumerable<TramoDto> GetAll(TramoFiltroDto filtro);
        TramoDto GetByCoordinates(string coordinates);
        long CreateOrUpdate(TramoDto dto);
        long UpdateRouting(TramoDto dto);
    }
}
