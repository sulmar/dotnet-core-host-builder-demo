using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace dotnet_core_host_builder_demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // add package Microsoft.Extensions.Hosting
            IHost host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IHostedService, MyTcpServer>();
                    services.AddScoped<IRequestService, MyRequestService>();

                    // Konfiguracja w kodzie
                    // services.Configure<MyOptions>(option => option.Port = 8899);

                    // Pobieranie konfiguracji z pliku
                    services.AddOptions();
                    services.Configure<MyOptions>(hostContext.Configuration.GetSection("MyOptions"));
                })
               
                 .ConfigureAppConfiguration((hostContext, config) =>
                 {
                     // add package Microsoft.Extensions.Configuration.Json;
                     config.AddJsonFile("appsettings.json", optional: true);
                 })
                 .ConfigureLogging((hostContext, configLogging) =>
                   {
                       // add package Microsoft.Extensions.Logging.Console
                       configLogging.AddConsole();

                       // Microsoft.Extensions.Logging.Debug
                       configLogging.AddDebug();
                   })
                .Build();

            await host.RunAsync();
        }
    }

}
