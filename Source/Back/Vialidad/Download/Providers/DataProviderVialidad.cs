using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class DataProviderVialidad : DataProviderBase
    {
        #region Constats
        private const string _Url = "http://www2.vialidad.gob.ar/dnv-estado-de-rutas?shs_term_node_tid_depth=All";
        #endregion

        #region Constructors
        public DataProviderVialidad()
            : base() { }
        #endregion

        #region Overrides
        public override List<TramoImport> GetInfo()
        {
            var items = new List<TramoImport>();
            try
            {
                for (int iPos = 0; iPos <= 31; iPos++)
                {
                    string url = _Url;
                    if (iPos > 0)
                        url = $"{url}&page={iPos}";

                    WebClient wc = new WebClient();
                    byte[] response = wc.DownloadData(url);
                    string htmlContent = Encoding.UTF8.GetString(response);
                    if (string.IsNullOrEmpty(htmlContent))
                        continue;

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    List<HtmlNode> rows = htmlDocument.DocumentNode.Descendants("tr").ToList();
                    foreach (HtmlNode itemRow in rows)
                    {
                        List<HtmlNode> cols = itemRow.Descendants("td").ToList();
                        if (cols.Count == 0) continue;

                        TramoImport itemToAdd = new TramoImport();
                        itemToAdd.Provincia = cols[0].InnerText.Trim();
                        itemToAdd.Ruta = cols[1].InnerText.Trim();
                        itemToAdd.TramoNormalizado = cols[2].InnerText.Trim();
                        itemToAdd.TramoDesnormalizado = cols[2].InnerText.Trim();
                        itemToAdd.Calzada = cols[3].InnerText.Trim();
                        itemToAdd.Detalle = cols[4].InnerText.Trim();
                        itemToAdd.Observaciones = cols[5].InnerText.Trim();
                        itemToAdd.Actualizacion = cols[6].InnerText.Trim().StrToDateTime();

                        items.Add(itemToAdd);
                    }
                }

                _logger.Info("DataProviderVialidad.GetInfo", $"{items.Count} registros obtenidos.");
            }
            catch (Exception ex)
            {
                _logger.Error("DataProviderVialidad.GetInfo", ex.Message, ex);
            }
            return items;
        }
        #endregion
    }
}
