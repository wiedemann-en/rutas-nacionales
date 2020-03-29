using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;

namespace Vialidad.Contracts.Services
{
    public interface IServiceProvincia
    {
        ProvinciaDto GetByKey(string key);
        IEnumerable<ProvinciaDto> GetAll(bool withTramos);
        IEnumerable<ProvinciaDto> GetAllByRuta(int idRuta);
        int Create(ProvinciaDto dto);
    }
}
