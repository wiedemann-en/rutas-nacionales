using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Vialidad.Cache;
using Vialidad.Cache.Infrastructure;
using Vialidad.Contracts.Services;
using Vialidad.Logger.Infrastructure;
using Vialidad.Logger.Interfaces;
using Vialidad.Logger.Logic;
using Vialidad.Services;

namespace Vialidad.Web.Api.App_Start
{
    public class WebApiAutofac
    {
        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            //Register your Web API controllers.  
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //Verificamos si debemos utilizar cache
            if (CacheCommonSettings.CacheActive)
            {
                builder.RegisterType<ServiceProvinciaCache>()
                       .As<IServiceProvincia>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceReferenciaCache>()
                       .As<IServiceReferencia>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceRutaCache>()
                       .As<IServiceRuta>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceCalzadaCache>()
                       .As<IServiceCalzada>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceTramoCache>()
                       .As<IServiceTramo>()
                       .InstancePerRequest();
            }
            else
            {
                builder.RegisterType<ServiceProvincia>()
                       .As<IServiceProvincia>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceReferencia>()
                       .As<IServiceReferencia>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceRuta>()
                       .As<IServiceRuta>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceCalzada>()
                       .As<IServiceCalzada>()
                       .InstancePerRequest();

                builder.RegisterType<ServiceTramo>()
                       .As<IServiceTramo>()
                       .InstancePerRequest();
            }

            //Registramos logger
            if (LoggerCommonSettings.LogMode.ToUpper() == "DB")
            {
                builder.RegisterType<LoggerDb>()
                   .As<ILogger>()
                   .InstancePerRequest();
            }
            else
            {
                builder.RegisterType<LoggerFile>()
                   .As<ILogger>()
                   .InstancePerRequest();
            }

            //Set the dependency resolver to be Autofac.  
            Container = builder.Build();

            return Container;
        }
    }
}