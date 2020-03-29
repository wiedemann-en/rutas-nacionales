using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Models;

namespace Vialidad.Download.Providers
{
    public abstract class DataProviderBase
    {
        #region Protected Attributes
        protected readonly ILogger _logger;
        #endregion

        #region Constructors
        public DataProviderBase()
        {
            _logger = LoggerFactory.GetInstance();
        }
        #endregion

        #region Abstract Methods
        public abstract List<TramoImport> GetInfo();
        #endregion
    }
}
