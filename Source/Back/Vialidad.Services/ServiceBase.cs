using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Model;

namespace Vialidad.Services
{
    public abstract class ServiceBase : IDisposable
    {
        #region Properties
        protected readonly VialidadContext _dbContext;
        protected readonly ILogger _logger;
        #endregion

        #region Constructors
        public ServiceBase()
        {
            _dbContext = new VialidadContext();
            _logger = LoggerFactory.GetInstance();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        #endregion
    }
}
