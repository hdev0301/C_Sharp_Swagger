using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Siemplify.Server.WebApi.Infrastructure;

namespace Siemplify.Server.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(config => WebApiConfig.Configure(config, new SelfHostingParameters(null)));
        }
    }
}
