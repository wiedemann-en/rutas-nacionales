using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;

namespace Vialidad.Contracts.Services
{
    public interface IServiceRuta
    {
        RutaDto GetByKey(string key);
        IEnumerable<RutaDto> GetAll(bool withTramos);
        IEnumerable<RutaDto> GetAllByProvincia(int idProvincia);
        int Create(RutaDto dto);
    }
}
