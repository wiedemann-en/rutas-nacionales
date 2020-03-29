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
    public class ServiceReferenciaCache : IServiceReferencia
    {
        #region Private Attributes
        private readonly IServiceReferencia _service;
        #endregion

        #region Constructors
        public ServiceReferenciaCache()
        {
            _service = new ServiceReferencia();
        }
        #endregion

        #region IServiceReferencia
        public IEnumerable<ReferenciaDto> GetAll()
        {
            string cacheKey = string.Format("ServiceReferencia@GetAll");
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAll();
            });
        }
        #endregion
    }
}
