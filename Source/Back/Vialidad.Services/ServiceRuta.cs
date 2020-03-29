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
    public class ServiceRuta : ServiceBase, IServiceRuta, IDisposable
    {
        #region Constructors
        public ServiceRuta()
            : base() { }
        #endregion

        #region IServiceRuta
        public RutaDto GetByKey(string key)
        {
            RutaDto result = null;
            try
            {
                RutaEntity rutaDb = _dbContext.RutaDataSet.FirstOrDefault(x => x.Key == key);
                if (rutaDb != null)
                    result = MapEntityToDto.Map(rutaDb);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceRuta.GetByKey", ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<RutaDto> GetAll(bool withTramos)
        {
            List<RutaDto> result = new List<RutaDto>();
            try
            {
                IEnumerable<RutaEntity> rutas = _dbContext.RutaDataSet.ToList();
                if (withTramos)
                {
                    foreach (var itemRuta in rutas)
                    {
                        bool isValid = _dbContext.TramoDataSet.Any(x =>
                            x.IdRuta == itemRuta.IdRuta &&
                            !string.IsNullOrEmpty(x.Coordenadas) &&
                            x.Activo);

                        if (isValid)
                            result.Add(MapEntityToDto.Map(itemRuta));
                    }
                }
                else
                {
                    result = rutas.Select(x => MapEntityToDto.Map(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceRuta.GetAll", ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<RutaDto> GetAllByProvincia(int idProvincia)
        {
            List<RutaDto> result = new List<RutaDto>();
            try
            {
                IEnumerable<RutaEntity> rutas = _dbContext.RutaDataSet.ToList();
                foreach (var itemRuta in rutas)
                {
                    bool isValid = _dbContext.TramoDataSet.Any(x =>
                        x.IdProvincia == idProvincia &&
                        x.IdRuta == itemRuta.IdRuta &&
                        !string.IsNullOrEmpty(x.Coordenadas) &&
                        x.Activo);

                    if (isValid)
                        result.Add(MapEntityToDto.Map(itemRuta));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceRuta.GetAllByProvincia", ex.Message, ex);
            }
            return result;
        }

        public int Create(RutaDto dto)
        {
            int result = default(int);
            try
            {
                string key = NormalizerKey.Normalize(dto.Nombre);
                RutaEntity rutaDb = _dbContext.RutaDataSet.FirstOrDefault(x => x.Key == key);
                if (rutaDb == null)
                    return result;

                rutaDb = MapDtoToEntity.Map(dto);
                _dbContext.RutaDataSet.Add(rutaDb);
                _dbContext.SaveChanges();

                result = rutaDb.IdRuta;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        _logger.Error("ServiceRuta.CreateOrUpdate", $"PropertyName: {ve.PropertyName} - ErrorMessage: {ve.ErrorMessage}", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceRuta.Create", ex.Message, ex);
            }
            return result;
        }
        #endregion
    }
}
