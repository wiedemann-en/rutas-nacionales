using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Cache.Infrastructure;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Services;

namespace Vialidad.Cache
{
    public class ServiceRutaCache : IServiceRuta
    {
        #region Private Attributes
        private readonly IServiceRuta _service;
        #endregion

        #region Constructors
        public ServiceRutaCache()
        {
            _service = new ServiceRuta();
        }
        #endregion

        #region IServiceRuta
        public int Create(RutaDto dto)
        {
            int result = _service.Create(dto);
            SimpleCache.CleanCache();
            return result;
        }

        public IEnumerable<RutaDto> GetAll(bool withTramos)
        {
            string cacheKey = string.Format("ServiceRuta@GetAll@{0}", withTramos ? "ConTramos" : "SinTramos");
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAll(withTramos);
            });
        }

        public IEnumerable<RutaDto> GetAllByProvincia(int idProvincia)
        {
            string cacheKey = string.Format("ServiceRuta@GetAllByProvincia@{0}", idProvincia);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAllByProvincia(idProvincia);
            });
        }

        public RutaDto GetByKey(string key)
        {
            string cacheKey = string.Format("ServiceRuta@GetByKey@{0}", key);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetByKey(key);
            });
        }
        #endregion
    }
}
