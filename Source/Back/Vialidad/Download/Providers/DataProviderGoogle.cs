using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Models;
using Vialidad.Utils;
using Vialidad.Utils.Extensions;

namespace Vialidad.Download.Providers
{
    public class DataProviderGoogle : DataProviderBase
    {
        #region Constants
        private const string _Url = "https://spreadsheets.google.com/feeds/list/17AqjqeNvM4nG6cOUsUFKFaKXMiNmztYfzHIxeM9FcXk/1/public/values?alt=json";
        #endregion

        #region Constructors
        public DataProviderGoogle()
            : base() { }
        #endregion

        #region Overrides
        public override List<TramoImport> GetInfo()
        {
            var items = new List<TramoImport>();
            try
            {
                WebClient wc = new WebClient();
                byte[] response = wc.DownloadData(_Url);

                string jsonContent = Encoding.UTF8.GetString(response);
                if (string.IsNullOrEmpty(jsonContent))
                    return items;

                JObject jsonObj = JObject.Parse(jsonContent);
                JToken rows = jsonObj["feed"]["entry"];

                foreach (var itemRow in rows)
                {
                    TramoImport itemToAdd = new TramoImport();
                    itemToAdd.Provincia = GetValue(itemRow, "provincia");

                    if (itemToAdd.Provincia.ToUpper() == "SANTA CRUZ")
                    {
                    }

                    itemToAdd.Ruta = GetValue(itemRow, "ruta");
                    itemToAdd.TramoNormalizado = GetValue(itemRow, "tramo");
                    itemToAdd.TramoDesnormalizado = GetValue(itemRow, "tramo");
                    itemToAdd.Calzada = GetValue(itemRow, "calzada");
                    itemToAdd.Detalle = GetValue(itemRow, "detalle");
                    itemToAdd.Observaciones = GetValue(itemRow, "observaciones");
                    itemToAdd.Actualizacion = GetValueDateTime(itemRow, "actualizacion");

                    //Hay algún error con la fecha de actualización
                    if (itemToAdd.Actualizacion == DateTime.MinValue)
                        itemToAdd.Actualizacion = GetValueDateTime(itemRow, "_ckd7g");

                    items.Add(itemToAdd);
                }

                _logger.Info("DataProviderGoogle.GetInfo", $"{items.Count} registros obtenidos.");
            }
            catch (Exception ex)
            {
                _logger.Error("DataProviderGoogle.GetInfo", ex.Message, ex);
            }
            return items;
        }
        #endregion

        #region Helpers
        private string GetValue(JToken jsonToken, string propertyName)
        {
            var value = string.Empty;
            try
            {
                if (jsonToken[$"gsx${propertyName}"] != null)
                {
                    value = (string)jsonToken[$"gsx${propertyName}"]["$t"];
                    if (!string.IsNullOrEmpty(value))
                        value = value.Trim();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("DataProviderGoogle.GetValue", "PropertyName: " + propertyName, ex);
            }
            return value;
        }
        private DateTime GetValueDateTime(JToken jsonToken, string propertyName)
        {
            var value = GetValue(jsonToken, propertyName);
            if (string.IsNullOrEmpty(value))
                return DateTime.MinValue;

            //24/01/2020 - 07:10

            //Normalizamos algunos valores
            value = value.ToLower();
            value = value.Replace("a.m.", "am");
            value = value.Replace("a. m.", "am");
            value = value.Replace("a.m", "am");
            value = value.Replace("a. m", "am");
            value = value.Replace("a:m", "am");
            value = value.Replace("a: m", "am");
            value = value.Replace("p.m.", "pm");
            value = value.Replace("p. m.", "pm");
            value = value.Replace("p.m", "pm");
            value = value.Replace("p. m", "pm");
            value = value.Replace("p:m", "pm");
            value = value.Replace("p: m", "pm");
            value = value.Replace("hs.", "hs");

            var currentYear = DateTime.Now.Year.ToString();
            var date = string.Empty;
            var time = string.Empty;

            if (value.Contains(","))
            {
                date = value.Split(',')[0];
                time = value.Split(',')[1];
            }
            else if (value.Contains("."))
            {
                date = value.Split('.')[0];
                time = value.Split('.')[1];
            }
            else if (value.Contains(" - "))
            {
                date = value.Split('-')[0];
                time = value.Split('-')[1];
            }
            else if (value.Contains(currentYear))
            {
                var indexSplit = value.IndexOf(currentYear) + currentYear.Length;
                date = value.Substring(0, indexSplit);
                time = value.Substring(indexSplit);
            }

            if ((!string.IsNullOrEmpty(date)) && (!string.IsNullOrEmpty(time)))
            {
                date = date.Replace("1ro", "01");
                date = date.Replace("enero", "/01/");
                date = date.Replace("febrero", "/02/");
                date = date.Replace("marzo", "/03/");
                date = date.Replace("abril", "/04/");
                date = date.Replace("mayo", "/05/");
                date = date.Replace("junio", "/06/");
                date = date.Replace("julio", "/07/");
                date = date.Replace("agosto", "/08/");
                date = date.Replace("setiembre", "/09/");
                date = date.Replace("septiembre", "/09/");
                date = date.Replace("octubre", "/10/");
                date = date.Replace("noviembre", "/11/");
                date = date.Replace("diciembre", "/12/");
                date = date.Replace("de", "/");
                date = date.Replace(" ", string.Empty);
                date = date.Replace("//", "/");

                if (date.Split('/').Length == 3)
                {
                    var day = date.Split('/')[0].Trim().PadLeft(2, '0');
                    var month = date.Split('/')[1].Trim().PadLeft(2, '0');
                    var year = date.Split('/')[2].Trim().PadLeft(4, '0');
                    date = $"{day}/{month}/{year}";
                }

                time = time.Replace(".", ":");
                time = time.Replace("am", string.Empty);
                time = time.Replace("pm", string.Empty);
                time = time.Replace("hs", string.Empty);
                time = time.Trim();

                if (time.StartsWith(":"))
                    time = time.Substring(1);
                if (time.EndsWith(":"))
                    time = time.Substring(0, time.Length - 1);

                if (time.Split(':').Length == 2)
                {
                    var hour = time.Split(':')[0].Trim().PadLeft(2, '0');
                    var minute = time.Split(':')[1].Trim().PadLeft(2, '0');
                    time = $"{hour}:{minute}";
                    time = time.Trim();
                }
                else if (time.Split(' ').Length == 2)
                {
                    var hour = time.Split(' ')[0].Trim().PadLeft(2, '0');
                    var minute = time.Split(' ')[1].Trim().PadLeft(2, '0');
                    time = $"{hour}:{minute}";
                    time = time.Trim();
                }

                value = $"{date.Trim()} {time.Trim()}";
            }

            var result = value.StrToDateTime();
            return result;
        }
        #endregion
    }
}
