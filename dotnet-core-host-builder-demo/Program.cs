using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace dotnet_core_host_builder_demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).UseTcpService(request => $"ECHO {request}").Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder().ConfigureAppConfiguration((hostContext, config) =>
                  {
                      config.AddJsonFile("appsettings.json", optional: true);
                  })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                });
    }



}

