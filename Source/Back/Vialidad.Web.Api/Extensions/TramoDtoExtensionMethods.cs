using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vialidad.Contracts.Models;
using Vialidad.Services.Normalizer;
using Vialidad.Web.Api.Models.Logic;

namespace Vialidad.Web.Api.Extensions
{
    public static class TramoDtoExtensionMethods
    {
        public static List<ReferenciaModel> GetListReferencias(this TramoDto dto, IEnumerable<ReferenciaDto> referencias)
        {
            var result = new List<ReferenciaModel>();

            var detalleFull = $"{dto.Detalle} {dto.Observaciones}";
            detalleFull = NormalizerKey.Normalize(detalleFull);

            foreach (var itemReferencia in referencias)
            {
                if (string.IsNullOrEmpty(itemReferencia.PalabrasClaves))
                    continue;

                var keys = itemReferencia.PalabrasClaves.Split('|');
                foreach (var itemKey in keys)
                {
                    if (!detalleFull.Contains(itemKey))
                        continue;

                    var referenciaExists = result.FirstOrDefault(x => x.ImageName == itemReferencia.Imagen);
                    if (referenciaExists == null)
                    {
                        var imageName = itemReferencia.Imagen;
                        if (imageName.EndsWith(".png"))
                        {
                            imageName = imageName.Split('.')[0];
                            imageName = $"{imageName}_transparent.png";
                        }
                        var imagePath = $"images/referencias/{itemReferencia.Tipo.ToLower()}/{imageName}";

                        var referenciaToAdd = new ReferenciaModel();
                        referenciaToAdd.ImageName = itemReferencia.Imagen;
                        referenciaToAdd.ImagePath = imagePath;
                        referenciaToAdd.ImageDesc = itemReferencia.Nombre;
                        referenciaToAdd.ImageWidth = itemReferencia.Ancho;
                        referenciaToAdd.ImageHeight = itemReferencia.Alto;
                        result.Add(referenciaToAdd);
                    }
                    else if (!referenciaExists.ImageDesc.Contains(itemReferencia.Nombre))
                    {
                        referenciaExists.ImageDesc = $"{referenciaExists.ImageDesc} - {itemReferencia.Nombre}";
                    }
                }
            }

            return result;
        }

        public static List<CoordenadaModel> GetListCoordenadas(this TramoDto dto)
        {
            var result = new List<CoordenadaModel>();
            if (string.IsNullOrEmpty(dto.Coordenadas))
                return result;

            string[] coordSplit = dto.Coordenadas.Split('/');
            foreach (var itemCoordenada in coordSplit)
            {
                if (itemCoordenada.Split(',').Length != 2)
                    continue;

                var coordenadaToAdd = new CoordenadaModel();
                coordenadaToAdd.Latitud = itemCoordenada.Split(',')[0];
                coordenadaToAdd.Longitud = itemCoordenada.Split(',')[1];
                result.Add(coordenadaToAdd);
            }

            return result;
        }

        public static string GetColorRuta(this TramoDto dto)
        {
            var _replaces = new Dictionary<string, string>();
            _replaces.Add("Á", "A");
            _replaces.Add("É", "E");
            _replaces.Add("Í", "I");
            _replaces.Add("Ó", "O");
            _replaces.Add("Ú", "U");

            var _keyWords = new Dictionary<string, List<string>>();
            _keyWords.Add("#00AF2A", new List<string>() { "NORMAL", "TRANSITABLE", "BUENO", "PRECAUCION", "SIN DEMARCACION", "INCOMPLETO", "NIEBLA", "HUMEDA", "LLUVIA", "PRECIPITACIONES" });
            //_keyWords.Add("#FFE000", new List<string>() { "PRECAUCION", "SIN DEMARCACION", "INCOMPLETO", "NIEBLA", "HUMEDA", "LLUVIA", "PRECIPITACIONES" });
            _keyWords.Add("#FFA600", new List<string>() { "REGULAR", "DETERIORADA", "POCEADA", "BACHES", "MALO", "CORTE PARCIAL", "COMPROMETIDO", "MAXIMA PRECAUCION", "DESVIO", "ALERTA" });
            _keyWords.Add("#FF0000", new List<string>() { "INTRANSITABLE", "INTERRUMPIDA", "CLAUSURADA", "4X4" }); //"HUELLONES"

            string desc = dto.Detalle.ToUpper();
            foreach (var itemReplace in _replaces)
                desc = desc.Replace(itemReplace.Key, itemReplace.Value);

            string color = "#00AF2A";
            foreach (var itemKeyWord in _keyWords)
            {
                foreach (var item in itemKeyWord.Value)
                    if (desc.Contains(item))
                        color = itemKeyWord.Key;
            }

            return color;
        }

        public static string GetColorFechaActualizacion(this TramoDto dto)
        {
            string color = "#00AF2A";
            if (dto.FechaActualizacion < DateTime.Now.AddDays(-20))
                color = "#FF0000";
            else if (dto.FechaActualizacion < DateTime.Now.AddDays(-10))
                color = "#FFA600";
            return color;
        }
    }
}