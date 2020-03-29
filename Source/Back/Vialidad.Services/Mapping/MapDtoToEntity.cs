using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Model.DbModel;
using Vialidad.Services.Normalizer;

namespace Vialidad.Services.Mapping
{
    static class MapDtoToEntity
    {
        #region Provincia
        public static ProvinciaEntity Map(ProvinciaDto entidadDto)
        {
            var entity = new ProvinciaEntity();
            entity.IdProvincia = entidadDto.Id;
            entity.Nombre = entidadDto.Nombre;
            entity.Key = entidadDto.Key;
            entity.Coordenadas = entidadDto.Coordenadas;
            entity.ZoomInicial = entidadDto.ZoomInicial;
            return entity;
        }
        public static IEnumerable<ProvinciaEntity> Map(IEnumerable<ProvinciaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion

        #region Ruta
        public static RutaEntity Map(RutaDto entidadDto)
        {
            var entity = new RutaEntity();
            entity.IdRuta = entidadDto.Id;
            entity.Nombre = entidadDto.Nombre;
            entity.Key = entidadDto.Key;
            entity.Coordenadas = entidadDto.Coordenadas;
            entity.ZoomInicial = entidadDto.ZoomInicial;
            return entity;
        }
        public static IEnumerable<RutaEntity> Map(IEnumerable<RutaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion

        #region Calzada
        public static CalzadaEntity Map(CalzadaDto entidadDto)
        {
            var entity = new CalzadaEntity();
            entity.IdCalzada = entidadDto.Id;
            entity.Nombre = entidadDto.Nombre;
            entity.Key = entidadDto.Key;
            return entity;
        }
        public static IEnumerable<CalzadaEntity> Map(IEnumerable<CalzadaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion

        #region Tramo
        public static TramoEntity Map(TramoDto entidadDto)
        {
            var entity = new TramoEntity();
            entity.IdTramo = entidadDto.IdTramo;
            entity.IdProvincia = entidadDto.IdProvincia;
            entity.IdRuta = entidadDto.IdRuta;
            entity.IdCalzada = entidadDto.IdCalzada;
            entity.TramoNormalizado = entidadDto.TramoNormalizado;
            entity.TramoDesnormalizado = entidadDto.TramoDesnormalizado;
            entity.Coordenadas = entidadDto.Coordenadas;
            entity.Detalle = entidadDto.Detalle;
            entity.Observaciones = entidadDto.Observaciones;
            entity.FechaActualizacion = entidadDto.FechaActualizacion;
            entity.Orden = entidadDto.Orden;
            return entity;
        }
        public static IEnumerable<TramoEntity> Map(IEnumerable<TramoDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion
    }
}
