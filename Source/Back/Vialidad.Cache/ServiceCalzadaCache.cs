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
    public class ServiceCalzadaCache : IServiceCalzada
    {
        #region Private Attributes
        private readonly IServiceCalzada _service;
        #endregion

        #region Constructors
        public ServiceCalzadaCache()
        {
            _service = new ServiceCalzada();
        }
        #endregion

        #region IServiceCalzada
        public int Create(CalzadaDto dto)
        {
            int result = _service.Create(dto);
            SimpleCache.CleanCache();
            return result;
        }

        public IEnumerable<CalzadaDto> GetAll(bool withTramos)
        {
            string cacheKey = string.Format("ServiceCalzada@GetAll@{0}", withTramos ? "ConTramos" : "SinTramos");
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAll(withTramos);
            });
        }

        public CalzadaDto GetByKey(string key)
        {
            string cacheKey = string.Format("ServiceCalzada@GetByKey@{0}", key);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetByKey(key);
            });
        }
        #endregion
    }
}
