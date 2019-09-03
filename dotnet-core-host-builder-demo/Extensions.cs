using System;
using System.Threading.Tasks;
using dotnet_core_host_builder_demo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IHostBuilder UseHostedService<T>(this IHostBuilder hostBuilder)
        where T : class, IHostedService, IDisposable
    {
        return hostBuilder.ConfigureServices(services =>
            services.AddHostedService<T>());
    }

    public static IHostBuilder UseTcpService(this IHostBuilder hostBuilder, RequestTcp request)
    {
        return hostBuilder.UseHostedService<MyTcpServer>()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<RequestTcp>(content => request(content));

                // Domyślna konfiguracja zdefiniowana za pomocą kodu
                 services.Configure<MyOptions>(option => option.Port = 5000);

                // Pobieranie konfiguracji z pliku
                services.AddOptions();
                services.Configure<MyOptions>(hostContext.Configuration.GetSection("MyOptions"));

            });
    }

    public static IHostBuilder UseTcpService(this IHostBuilder hostBuilder, IRequestService requestService)
    {
        return hostBuilder.UseHostedService<MyTcpServer>()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<RequestTcp>(content => requestService.Send(content));

                // Domyślna konfiguracja zdefiniowana za pomocą kodu
                services.Configure<MyOptions>(option => option.Port = 5000);

                // Pobieranie konfiguracji z pliku
                services.AddOptions();
                services.Configure<MyOptions>(hostContext.Configuration.GetSection("MyOptions"));

            });
    }
}