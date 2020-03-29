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
    public class ServiceCalzada : ServiceBase, IServiceCalzada, IDisposable
    {
        #region Constructors
        public ServiceCalzada()
            : base() { }
        #endregion

        #region IServiceCalzada
        public CalzadaDto GetByKey(string key)
        {
            CalzadaDto result = null;
            try
            {
                CalzadaEntity calzadaDb = _dbContext.CalzadaDataSet.FirstOrDefault(x => x.Key == key);
                if (calzadaDb != null)
                    result = MapEntityToDto.Map(calzadaDb);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceCalzada.GetByKey", ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<CalzadaDto> GetAll(bool withTramos)
        {
            List<CalzadaDto> result = new List<CalzadaDto>();
            try
            {
                IEnumerable<CalzadaEntity> calzadas = _dbContext.CalzadaDataSet.ToList();
                if (withTramos)
                {
                    foreach (var itemCalzada in calzadas)
                    {
                        bool isValid = _dbContext.TramoDataSet.Any(x =>
                            x.IdCalzada == itemCalzada.IdCalzada &&
                            !string.IsNullOrEmpty(x.Coordenadas) &&
                            x.Activo);

                        if (isValid)
                            result.Add(MapEntityToDto.Map(itemCalzada));
                    }
                }
                else
                {
                    result = calzadas.Select(x => MapEntityToDto.Map(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceCalzada.GetAll", ex.Message, ex);
            }
            return result;
        }

        public int Create(CalzadaDto dto)
        {
            int result = default(int);
            try
            {
                string key = NormalizerKey.Normalize(dto.Nombre);
                CalzadaEntity calzadaDb = _dbContext.CalzadaDataSet.FirstOrDefault(x => x.Key == key);
                if (calzadaDb != null)
                    return result;

                calzadaDb = MapDtoToEntity.Map(dto);
                _dbContext.CalzadaDataSet.Add(calzadaDb);
                _dbContext.SaveChanges();

                result = calzadaDb.IdCalzada;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        _logger.Error("ServiceCalzada.CreateOrUpdate", $"PropertyName: {ve.PropertyName} - ErrorMessage: {ve.ErrorMessage}", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceCalzada.Create", ex.Message, ex);
            }
            return result;
        }
        #endregion
    }
}
