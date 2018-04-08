using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemplify.Server.WebApi;
using Siemplify.Server.WebApi.Infrastructure;

namespace Server.Host
{
    public class Program
    {
        private static IDisposable _webApiService;

        static void Main(string[] args)
        {
            try
            {
                StartWebApiServices();

                Console.Beep(520, 115);
                Console.Beep(620, 165);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
            }
        }

        private static void StartWebApiServices()
        {
            var baseAddress = ConfigurationManager.AppSettings["ServerAddress"];
            var hostingParams = new SelfHostingParameters(baseAddress);
            _webApiService = WebApiConfig.SelfHost(hostingParams);
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

    }
}
