using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Model;
using Vialidad.Model.DbModel;
using Vialidad.Services.Mapping;
using Vialidad.Services.Normalizer;

namespace Vialidad.Services
{
    public class ServiceProvincia : ServiceBase, IServiceProvincia, IDisposable
    {
        #region Constructors
        public ServiceProvincia()
            : base() { }
        #endregion

        #region IServiceProvincia
        public ProvinciaDto GetByKey(string key)
        {
            ProvinciaDto result = null;
            try
            {
                ProvinciaEntity provinciaDb = _dbContext.ProvinciaDataSet.FirstOrDefault(x => x.Key == key);
                if (provinciaDb != null)
                    result = MapEntityToDto.Map(provinciaDb);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceProvincia.GetByKey", ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<ProvinciaDto> GetAll(bool withTramos)
        {
            List<ProvinciaDto> result = new List<ProvinciaDto>();
            try
            {
                IEnumerable<ProvinciaEntity> provincias = _dbContext.ProvinciaDataSet.ToList();
                if (withTramos)
                {
                    foreach (var itemProvincia in provincias)
                    {
                        bool isValid = _dbContext.TramoDataSet.Any(x =>
                            x.IdProvincia == itemProvincia.IdProvincia &&
                            !string.IsNullOrEmpty(x.Coordenadas) &&
                            x.Activo);

                        if (isValid)
                            result.Add(MapEntityToDto.Map(itemProvincia));
                    }
                }
                else
                {
                    result = provincias.Select(x => MapEntityToDto.Map(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceProvincia.GetAll", ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<ProvinciaDto> GetAllByRuta(int idRuta)
        {
            List<ProvinciaDto> result = new List<ProvinciaDto>();
            try
            {
                IEnumerable<ProvinciaEntity> provincias = _dbContext.ProvinciaDataSet.ToList();
                foreach (var itemProvincia in provincias)
                {
                    bool isValid = _dbContext.TramoDataSet.Any(x =>
                        x.IdRuta == idRuta &&
                        x.IdProvincia == itemProvincia.IdProvincia &&
                        !string.IsNullOrEmpty(x.Coordenadas) &&
                        x.Activo);

                    if (isValid)
                        result.Add(MapEntityToDto.Map(itemProvincia));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceProvincia.GetAllByRuta", ex.Message, ex);
            }
            return result;
        }

        public int Create(ProvinciaDto dto)
        {
            int result = default(int);
            try
            {
                string key = NormalizerKey.Normalize(dto.Nombre);
                ProvinciaEntity provinciaDb = _dbContext.ProvinciaDataSet.FirstOrDefault(x => x.Key == key);
                if (provinciaDb != null)
                    return result;

                provinciaDb = MapDtoToEntity.Map(dto);
                _dbContext.ProvinciaDataSet.Add(provinciaDb);
                _dbContext.SaveChanges();

                result = provinciaDb.IdProvincia;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        _logger.Error("ServiceProvincia.CreateOrUpdate", $"PropertyName: {ve.PropertyName} - ErrorMessage: {ve.ErrorMessage}", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceProvincia.Create", ex.Message, ex);
            }
            return result;
        }
        #endregion
    }
}
