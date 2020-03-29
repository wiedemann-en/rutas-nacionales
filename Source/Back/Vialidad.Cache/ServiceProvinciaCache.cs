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
    public class ServiceProvinciaCache : IServiceProvincia
    {
        #region Private Attributes
        private readonly IServiceProvincia _service;
        #endregion

        #region Constructors
        public ServiceProvinciaCache()
        {
            _service = new ServiceProvincia();
        }
        #endregion

        #region IServiceProvincia
        public int Create(ProvinciaDto dto)
        {
            int result = _service.Create(dto);
            SimpleCache.CleanCache();
            return result;
        }

        public IEnumerable<ProvinciaDto> GetAll(bool withTramos)
        {
            string cacheKey = string.Format("ServiceProvincia@GetAll@{0}", withTramos ? "ConTramos" : "SinTramos");
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAll(withTramos);
            });
        }

        public IEnumerable<ProvinciaDto> GetAllByRuta(int idRuta)
        {
            string cacheKey = string.Format("ServiceProvincia@GetAllByRuta@{0}", idRuta);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAllByRuta(idRuta);
            });
        }

        public ProvinciaDto GetByKey(string key)
        {
            string cacheKey = string.Format("ServiceProvincia@GetByKey@{0}", key);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetByKey(key);
            });
        }
        #endregion
    }
}
