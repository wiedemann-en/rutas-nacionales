using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;

namespace Vialidad.Contracts.Services
{
    public interface IServiceCalzada
    {
        CalzadaDto GetByKey(string key);
        IEnumerable<CalzadaDto> GetAll(bool withTramos);
        int Create(CalzadaDto dto);
    }
}
