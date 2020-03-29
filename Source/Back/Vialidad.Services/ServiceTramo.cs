using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Model;
using Vialidad.Model.DbModel;
using Vialidad.Services.Mapping;

namespace Vialidad.Services
{
    public class ServiceTramo : ServiceBase, IServiceTramo, IDisposable
    {
        #region Constructors
        public ServiceTramo()
            : base() { }
        #endregion

        #region IServiceTramo
        public IEnumerable<TramoDto> GetAll(bool onlyActives = true)
        {
            IEnumerable<TramoDto> result = new List<TramoDto>();
            try
            {
                IEnumerable<TramoEntity> tramos = _dbContext.TramoDataSet.AsEnumerable();
                if (onlyActives)
                    tramos = tramos.Where(x => x.Activo);

                result = MapEntityToDto.Map(tramos);

                //Ordenamos el resultado
                result = result
                    .OrderBy(x => x.IdProvincia)
                    .ThenBy(x => x.IdRuta)
                    .ThenBy(x => x.Orden)
                    .ThenBy(x => x.TramoNormalizado)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceTramo.GetAll", ex.Message, ex);
            }
            return result;
        }

        public IEnumerable<TramoDto> GetAll(TramoFiltroDto filtro)
        {
            IEnumerable<TramoDto> result = new List<TramoDto>();
            try
            {
                IEnumerable<TramoEntity> tramos = _dbContext.TramoDataSet
                    .Where(x => x.Activo).AsEnumerable();

                if (filtro.IdProvincia > 0)
                    tramos = tramos.Where(x => x.IdProvincia == filtro.IdProvincia);
                if (filtro.IdRuta > 0)
                    tramos = tramos.Where(x => x.IdRuta == filtro.IdRuta);
                if (filtro.IdCalzada > 0)
                    tramos = tramos.Where(x => x.IdCalzada == filtro.IdCalzada);

                result = MapEntityToDto.Map(tramos);

                //Ordenamos el resultado
                result = result
                    .OrderBy(x => x.IdProvincia)
                    .ThenBy(x => x.IdRuta)
                    .ThenBy(x => x.Orden)
                    .ThenBy(x => x.TramoNormalizado)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceTramo.GetAll", ex.Message, ex);
            }
            return result;
        }

        public TramoDto GetByCoordinates(string coordinates)
        {
            TramoDto result = null;
            try
            {
                TramoEntity tramo = _dbContext.TramoDataSet
                    .OrderByDescending(x => x.FechaActualizacion)
                    .ThenBy(x => x.Orden)
                    .FirstOrDefault(x => 
                        x.Coordenadas == coordinates && 
                        x.Activo);

                if (tramo != null)
                    result = MapEntityToDto.Map(tramo);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceTramo.GetByCoordinates", ex.Message, ex);
            }
            return result;
        }

        public long CreateOrUpdate(TramoDto dto)
        {
            long result = default(long);
            try
            {
                using (var dbContext = new VialidadContext())
                {
                    TramoEntity tramoDb = dbContext.TramoDataSet.AsNoTracking().FirstOrDefault(x =>
                        x.IdProvincia == dto.IdProvincia &&
                        x.IdRuta == dto.IdRuta &&
                        x.TramoNormalizado.ToUpper() == dto.TramoNormalizado.ToUpper());

                    if (tramoDb == null)
                    {
                        tramoDb = MapDtoToEntity.Map(dto);
                        tramoDb.FechaAlta = DateTime.Now;
                        tramoDb.Activo = false;

                        dbContext.TramoDataSet.Add(tramoDb);
                    }
                    else
                    {
                        tramoDb.IdCalzada = dto.IdCalzada;
                        tramoDb.Detalle = dto.Detalle;
                        tramoDb.Observaciones = dto.Observaciones;
                        tramoDb.FechaActualizacion = dto.FechaActualizacion;
                        tramoDb.TramoDesnormalizado = dto.TramoDesnormalizado;

                        var exist = dbContext.Set<TramoEntity>().Find(tramoDb.IdTramo);
                        if (exist != null)
                            dbContext.Entry<TramoEntity>(exist).CurrentValues.SetValues(tramoDb);
                    }

                    dbContext.SaveChanges();
                    result = tramoDb.IdTramo;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        _logger.Error("ServiceTramo.CreateOrUpdate", $"PropertyName: {ve.PropertyName} - ErrorMessage: {ve.ErrorMessage}", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceTramo.CreateOrUpdate", ex.Message, ex);
            }
            return result;
        }

        public long UpdateRouting(TramoDto dto)
        {
            long result = default(long);
            try
            {
                TramoEntity tramoDb = _dbContext.TramoDataSet.FirstOrDefault(x => x.IdTramo == dto.IdTramo);
                if (tramoDb != null)
                {
                    tramoDb.JsonRouting = dto.JsonRouting;
                    _dbContext.SaveChanges();
                }
                result = tramoDb.IdTramo;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                    foreach (var ve in eve.ValidationErrors)
                        _logger.Error("ServiceTramo.UpdateRouting", $"PropertyName: {ve.PropertyName} - ErrorMessage: {ve.ErrorMessage}", ex);
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceTramo.CreateOrUpdate", ex.Message, ex);
            }
            return result;
        }
        #endregion
    }
}
