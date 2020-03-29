using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Utils.Infrastructure;

namespace Vialidad.Scheduler.Infrastructure
{
    internal class SchedulerCommonSettings : CommonSettings
    {
        public static string SchedulerMode
        {
            get { return GetSettingDefault("SCHEDULER:Mode", "Interval"); }
        }
        public static int SchedulerIntervalMinutes
        {
            get { return int.Parse(GetSettingDefault("SCHEDULER:IntervalMinutes", "60")); }
        }
        public static DateTime SchedulerScheduledTime
        {
            get
            {
                var value = GetSettingDefault("SCHEDULER:ScheduledTime", "");
                if (!String.IsNullOrEmpty(value))
                    return DateTime.Parse(value);
                return DateTime.MinValue;
            }
        }
    }
}
