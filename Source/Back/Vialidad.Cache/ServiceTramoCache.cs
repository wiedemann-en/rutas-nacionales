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
    public class ServiceTramoCache : IServiceTramo
    {
        #region Private Attributes
        private readonly IServiceTramo _service;
        #endregion

        #region Constructors
        public ServiceTramoCache()
        {
            _service = new ServiceTramo();
        }
        #endregion

        #region IServiceTramo
        public long CreateOrUpdate(TramoDto dto)
        {
            long result = _service.CreateOrUpdate(dto);
            SimpleCache.CleanCache();
            return result;
        }

        public IEnumerable<TramoDto> GetAll(bool onlyActives = true)
        {
            string cacheKey = string.Format("ServiceTramo@GetAll@{0}", onlyActives);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAll(onlyActives);
            });
        }

        public IEnumerable<TramoDto> GetAll(TramoFiltroDto filtro)
        {
            string cacheKey = string.Format("ServiceTramo@GetAll@{0}@{1}@{2}", filtro.IdProvincia, filtro.IdRuta, filtro.IdCalzada);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetAll(filtro);
            });
        }

        public TramoDto GetByCoordinates(string coordinates)
        {
            string cacheKey = string.Format("ServiceTramo@GetByCoordinates@{0}", coordinates);
            return SimpleCache.GetCache(cacheKey, () =>
            {
                return _service.GetByCoordinates(coordinates);
            });
        }

        public long UpdateRouting(TramoDto dto)
        {
            long result = _service.UpdateRouting(dto);
            SimpleCache.CleanCache();
            return result;
        }
        #endregion
    }
}
