using System;
using System.Linq;
using System.Net;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Siemplify.Server.WebApi.Infrastructure;

namespace Siemplify.Server.WebApi
{
    public static class WebApiConfig
    {
        public static OAuthBearerAuthenticationOptions OAuthOptions { get; private set; }

        public static void Configure(HttpConfiguration config, SelfHostingParameters hostingParameters)
        {
            // Web API configuration and services

            config.EnableCors();

            // Enable this to see WebApi error log in output window
            //config.EnableSystemDiagnosticsTracing();

            if (hostingParameters.ControllerTypes != null)
            {
                config.Services.Replace(typeof(IHttpControllerTypeResolver), new SiemplifyControllerResolver(hostingParameters.ControllerTypes));
            }

            // Web API routes - loads all inheritance of ApiController (via reflection):
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
            );
            
            //config.SuppressDefaultHostAuthentication();

            config.MessageHandlers.Insert(0, new OwinServerCompressionHandler(new GZipCompressor()));
            //config.MessageHandlers.Add(new ResponseLogger());
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Formatters.Remove(config.Formatters.JsonFormatter);
            var dataContractJsonFormatter = new DataContractJsonFormatter();
            config.Formatters.Insert(0, dataContractJsonFormatter);
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = hostingParameters.UseDataContractSerializer;

            dataContractJsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("type", "json", new MediaTypeHeaderValue("application/json")));

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("type", "xml", new MediaTypeHeaderValue("application/xml")));

#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#endif

        }

        public static IDisposable SelfHost(SelfHostingParameters hostingParams)
        {

            var startupOptions = new StartOptions(hostingParams.BaseAddress);
            return WebApp.Start(startupOptions, appBuilder =>
            {
                var serverPort = GetPort(startupOptions.Urls.First());
                VerifyFreePort(serverPort);
                HttpListener listener = (HttpListener) appBuilder.Properties["System.Net.HttpListener"];
                listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication
                                                 | AuthenticationSchemes.Negotiate
                                                 | AuthenticationSchemes.Ntlm
                                                 | AuthenticationSchemes.Anonymous;


                OAuthOptions = new OAuthBearerAuthenticationOptions() {AuthenticationMode = AuthenticationMode.Active};

                appBuilder.UseOAuthBearerAuthentication(OAuthOptions);

                var config = new HttpConfiguration();


                Configure(config, hostingParams);
                appBuilder.UseWebApi(config);
                config.EnsureInitialized();
                var controllerSelectorService = config.Services.GetHttpControllerSelector();

                Console.WriteLine("======= Loading application server controllers");
                var controllers = controllerSelectorService.GetControllerMapping().Values;
                foreach (var controller in controllers)
                {
                    Console.WriteLine(string.Format("Loaded WebApi controller {0}.", controller.ControllerName));
                }

            });
        }

        private static int GetPort(string url)
        {
            return new Uri(url.Replace("*", "hostname")).Port;
        }

        private static void VerifyFreePort(int serverPort)
        {
            var activeListeners = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpListeners();
            var portListeners = activeListeners.Where(listener => listener.Port == serverPort);
            if (portListeners.Any())
            {
                throw new AddressAlreadyInUseException(string.Format("Server port {0} is already in use.", serverPort));
            }
        }

    }

    public class DataContractJsonFormatter : JsonMediaTypeFormatter
    {
        public override DataContractJsonSerializer CreateDataContractSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, new DataContractJsonSerializerSettings() {EmitTypeInformation = EmitTypeInformation.AsNeeded});
        }
    }
}