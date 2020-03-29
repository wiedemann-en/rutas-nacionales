using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vialidad.Fixture;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Scheduler.Infrastructure;
using Vialidad.Scheduler.Logic;

namespace Vialidad.Scheduler
{
    public partial class ServiceScheduler : ServiceBase
    {
        private readonly ILogger _logger;
        private Timer _schedular;

        public ServiceScheduler()
        {
            //Obtenemos instancia del logger
            _logger = LoggerFactory.GetInstance();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("ServicioScheduler", "Scraper Service Started.");
            SchedulerService();
        }

        protected override void OnStop()
        {
            _logger.Info("ServicioScheduler", "Scraper Service Stopped.");
            _schedular.Dispose();
        }

        #region Interfaz pública
        public void SchedulerService()
        {
            try
            {
                _schedular = new Timer(new TimerCallback(SchedularCallback));

                //Set the Default Time.
                var scheduler = SchedulerFactory.GetInstance();
                var scheduledTime = scheduler.GetScheduledTime();
                var timeSpan = scheduledTime.Subtract(DateTime.Now);

                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)",
                    timeSpan.Days,
                    timeSpan.Hours,
                    timeSpan.Minutes,
                    timeSpan.Seconds);

                _logger.Info("ServicioScheduler", string.Format("Service mode: {0}.", SchedulerCommonSettings.SchedulerMode));
                _logger.Info("ServicioScheduler", string.Format("Service to run after: {0}.", schedule));

                //Ejecutamos servicio que descarga la información de vialidad nacional
                var fixture = new ScraperFixture();
                fixture.ExecuteFixture();

                //Get the difference in Minutes between the Scheduled and Current Time.
                var dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                _schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                _logger.Error("ServicioScheduler", ex.Message, ex);
                StopWindowsService();
            }
        }
        #endregion

        #region Helpers
        private void SchedularCallback(object e)
        {
            SchedulerService();
        }
        private void StopWindowsService()
        {
            using (var serviceController = new ServiceController(SchedulerConstants.ServiceName))
            {
                serviceController.Stop();
            }
        }
        #endregion
    }
}
