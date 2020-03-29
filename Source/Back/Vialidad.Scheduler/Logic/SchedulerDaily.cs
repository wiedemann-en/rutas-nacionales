using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Scheduler.Infrastructure;
using Vialidad.Scheduler.Interfaces;

namespace Vialidad.Scheduler.Logic
{
    internal class SchedulerDaily : IScheduler
    {
        #region IScheduler
        public DateTime GetScheduledTime()
        {
            var scheduledTime = SchedulerCommonSettings.SchedulerScheduledTime;
            if (DateTime.Now > scheduledTime)
                scheduledTime = scheduledTime.AddDays(1);
            return scheduledTime;
        }
        #endregion
    }
}
