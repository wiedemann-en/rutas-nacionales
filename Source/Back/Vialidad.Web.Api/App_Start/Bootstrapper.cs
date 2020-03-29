using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Vialidad.Web.Api.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            WebApiAutofac.Initialize(GlobalConfiguration.Configuration);
        }
    }
}