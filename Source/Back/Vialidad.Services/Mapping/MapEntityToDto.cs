using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Model.DbModel;

namespace Vialidad.Services.Mapping
{
    static class MapEntityToDto
    {
        #region Provincia
        public static ProvinciaDto Map(ProvinciaEntity entity)
        {
            var entityDto = new ProvinciaDto();
            entityDto.Id = entity.IdProvincia;
            entityDto.Nombre = entity.Nombre;
            entityDto.Key = entity.Key;
            entityDto.Coordenadas = entity.Coordenadas;
            entityDto.ZoomInicial = entity.ZoomInicial;
            return entityDto;
        }
        public static IEnumerable<ProvinciaDto> Map(IEnumerable<ProvinciaEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        #endregion

        #region Referencia
        public static ReferenciaDto Map(ReferenciaEntity entity)
        {
            var entityDto = new ReferenciaDto();
            entityDto.IdReferencia = entity.IdReferencia;
            entityDto.Nombre = entity.Nombre;
            entityDto.Tipo = entity.Tipo;
            entityDto.Imagen = entity.Imagen;
            entityDto.PalabrasClaves = entity.PalabrasClaves;
            entityDto.Orden = entity.Orden.GetValueOrDefault(0);
            entityDto.Ancho = entity.Ancho.GetValueOrDefault(0);
            entityDto.Alto = entity.Alto.GetValueOrDefault(0);
            return entityDto;
        }
        public static IEnumerable<ReferenciaDto> Map(IEnumerable<ReferenciaEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        #endregion

        #region Ruta
        public static RutaDto Map(RutaEntity entity)
        {
            var entityDto = new RutaDto();
            entityDto.Id = entity.IdRuta;
            entityDto.Nombre = entity.Nombre;
            entityDto.Key = entity.Key;
            entityDto.Coordenadas = entity.Coordenadas;
            entityDto.ZoomInicial = entity.ZoomInicial;
            return entityDto;
        }
        public static IEnumerable<RutaDto> Map(IEnumerable<RutaEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        #endregion

        #region Calzada
        public static CalzadaDto Map(CalzadaEntity entity)
        {
            var entityDto = new CalzadaDto();
            entityDto.Id = entity.IdCalzada;
            entityDto.Nombre = entity.Nombre;
            entityDto.Key = entity.Key;
            return entityDto;
        }
        public static IEnumerable<CalzadaDto> Map(IEnumerable<CalzadaEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        #endregion

        #region Tramo
        public static TramoDto Map(TramoEntity entity)
        {
            var entityDto = new TramoDto();
            entityDto.IdTramo = entity.IdTramo;
            entityDto.IdProvincia = entity.IdProvincia;
            entityDto.IdRuta = entity.IdRuta;
            entityDto.IdCalzada = entity.IdCalzada;
            entityDto.TramoNormalizado = entity.TramoNormalizado;
            entityDto.TramoDesnormalizado = entity.TramoDesnormalizado;
            entityDto.Coordenadas = entity.Coordenadas;
            entityDto.Detalle = entity.Detalle;
            entityDto.Observaciones = entity.Observaciones;
            entityDto.FechaActualizacion = entity.FechaActualizacion;
            entityDto.Orden = entity.Orden;
            entityDto.JsonRouting = entity.JsonRouting;
            return entityDto;
        }
        public static IEnumerable<TramoDto> Map(IEnumerable<TramoEntity> entities)
        {
            return entities.Select(Map).ToList();
        }
        #endregion
    }
}
