using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Scheduler.Infrastructure;
using Vialidad.Scheduler.Interfaces;

namespace Vialidad.Scheduler.Logic
{
    internal class SchedulerInterval : IScheduler
    {
        #region IScheduler
        public DateTime GetScheduledTime()
        {
            int intervalMinutes = SchedulerCommonSettings.SchedulerIntervalMinutes;
            var scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
            if (DateTime.Now > scheduledTime)
                scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
            return scheduledTime;
        }
        #endregion
    }
}
