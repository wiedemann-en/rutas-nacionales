using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vialidad.Contracts.Models;
using Vialidad.Utils.Extensions;
using Vialidad.Web.Api.Extensions;
using Vialidad.Web.Api.Models.Logic;

namespace Vialidad.Web.Api.Mapping
{
    static class MapDtoToViewModel
    {
        #region Tramo
        public static TramoModel Map(TramoDto entidadDto)
        {
            var model = new TramoModel();
            model.IdTramo = entidadDto.IdTramo;
            model.IdProvincia = entidadDto.IdProvincia;
            model.IdRuta = entidadDto.IdRuta;
            model.IdCalzada = entidadDto.IdCalzada;
            model.Tramo = entidadDto.TramoNormalizado.ToTitleCase();
            model.Detalle = entidadDto.Detalle.ToTitleCase();
            model.Observaciones = entidadDto.Observaciones.ToTitleCase();
            model.FechaActualizacion = entidadDto.FechaActualizacion.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy HH:mm");
            model.Orden = entidadDto.Orden;
            model.Coordenadas = entidadDto.GetListCoordenadas();
            //model.Referencias = entidadDto.GetListReferencias();
            model.ColorRuta = entidadDto.GetColorRuta();
            model.ColorFechaActualizacion = entidadDto.GetColorFechaActualizacion();
            return model;
        }
        public static IEnumerable<TramoModel> Map(IEnumerable<TramoDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion

        #region Provincia
        public static DropDownItemModel Map(ProvinciaDto entidadDto)
        {
            var model = new DropDownItemModel();
            model.Id = entidadDto.Id.ToString();
            model.Nombre = entidadDto.Nombre;
            return model;
        }
        public static InfoMapModel MapInfo(ProvinciaDto entidadDto)
        {
            var model = new InfoMapModel();
            model.Id = entidadDto.Id.ToString();
            model.Nombre = entidadDto.Nombre;
            model.Zoom = entidadDto.ZoomInicial.GetValueOrDefault(5);
            model.Coordenadas = MapCoordenada(entidadDto.Coordenadas);
            return model;
        }
        public static IEnumerable<DropDownItemModel> Map(IEnumerable<ProvinciaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        public static IEnumerable<InfoMapModel> MapInfo(IEnumerable<ProvinciaDto> entidadesDto)
        {
            return entidadesDto.Select(MapInfo).ToList();
        }
        #endregion

        #region Ruta
        public static DropDownItemModel Map(RutaDto entidadDto)
        {
            var model = new DropDownItemModel();
            model.Id = entidadDto.Id.ToString();
            model.Nombre = entidadDto.Nombre;
            return model;
        }
        public static InfoMapModel MapInfo(RutaDto entidadDto)
        {
            var model = new InfoMapModel();
            model.Id = entidadDto.Id.ToString();
            model.Nombre = entidadDto.Nombre;
            model.Zoom = entidadDto.ZoomInicial.GetValueOrDefault(5);
            model.Coordenadas = MapCoordenada(entidadDto.Coordenadas);
            return model;
        }
        public static IEnumerable<DropDownItemModel> Map(IEnumerable<RutaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        public static IEnumerable<InfoMapModel> MapInfo(IEnumerable<RutaDto> entidadesDto)
        {
            return entidadesDto.Select(MapInfo).ToList();
        }
        #endregion

        #region Calzada
        public static DropDownItemModel Map(CalzadaDto entidadDto)
        {
            var model = new DropDownItemModel();
            model.Id = entidadDto.Id.ToString();
            model.Nombre = entidadDto.Nombre;
            return model;
        }
        public static IEnumerable<DropDownItemModel> Map(IEnumerable<CalzadaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion

        #region Referencia
        public static ReferenciaModel Map(ReferenciaDto entidadDto)
        {
            var model = new ReferenciaModel();
            model.ImageName = entidadDto.Nombre;
            model.ImagePath = entidadDto.Imagen;
            model.ImageDesc = entidadDto.Nombre;
            model.ImageWidth = entidadDto.Ancho;
            model.ImageHeight = entidadDto.Alto;
            return model;
        }
        public static IEnumerable<ReferenciaModel> Map(IEnumerable<ReferenciaDto> entidadesDto)
        {
            return entidadesDto.Select(Map).ToList();
        }
        #endregion

        #region Coordenada
        private static CoordenadaModel MapCoordenada(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (value.Split(',').Length != 2)
                return null;

            var model = new CoordenadaModel();
            model.Latitud = value.Split(',')[0];
            model.Longitud = value.Split(',')[1];
            return model;
        }
        #endregion
    }
}