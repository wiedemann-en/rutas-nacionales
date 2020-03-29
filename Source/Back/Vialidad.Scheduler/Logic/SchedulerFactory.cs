using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Scheduler.Infrastructure;
using Vialidad.Scheduler.Interfaces;

namespace Vialidad.Scheduler.Logic
{
    internal static class SchedulerFactory
    {
        #region Public Methods
        public static IScheduler GetInstance()
        {
            IScheduler result = null;
            switch (SchedulerCommonSettings.SchedulerMode.ToUpper())
            {
                case "DAILY":
                    result = new SchedulerDaily();
                    break;
                case "INTERVAL":
                    result = new SchedulerInterval();
                    break;
                default:
                    break;
            }
            return result;
        }
        #endregion
    }
}
