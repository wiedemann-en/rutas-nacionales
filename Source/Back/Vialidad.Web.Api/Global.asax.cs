using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Vialidad.Web.Api.App_Start;

namespace Vialidad.Web.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Bootstrapper.Run();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
