using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Models;
using Vialidad.Contracts.Services;
using Vialidad.Model.DbModel;
using Vialidad.Services.Mapping;

namespace Vialidad.Services
{
    public class ServiceReferencia : ServiceBase, IServiceReferencia, IDisposable
    {
        #region Constructors
        public ServiceReferencia()
            : base() { }
        #endregion

        #region IServiceCalzada
        public IEnumerable<ReferenciaDto> GetAll()
        {
            List<ReferenciaDto> result = new List<ReferenciaDto>();
            try
            {
                IEnumerable<ReferenciaEntity> referenciasTransito = _dbContext.ReferenciaDataSet
                    .Where(x => x.Tipo == "Transito")
                    .OrderBy(x => x.Orden)
                    .ToList();

                IEnumerable<ReferenciaEntity> referenciasClima = _dbContext.ReferenciaDataSet
                    .Where(x => x.Tipo == "Clima")
                    .OrderBy(x => x.Orden)
                    .ToList();

                result.AddRange(referenciasTransito.Select(x => MapEntityToDto.Map(x)).ToList());
                result.AddRange(referenciasClima.Select(x => MapEntityToDto.Map(x)).ToList());
            }
            catch (Exception ex)
            {
                _logger.Error("ServiceReferencia.GetAll", ex.Message, ex);
            }
            return result;
        }
        #endregion
    }
}
