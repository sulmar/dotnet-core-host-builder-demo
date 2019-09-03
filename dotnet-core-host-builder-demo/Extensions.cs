using System;
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

    public static IHostBuilder UseTcpService<T>(this IHostBuilder hostBuilder)
        where T : class, IRequestService
    {
        return hostBuilder.UseHostedService<MyTcpServer>()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddScoped<IRequestService, T>();

                // Konfiguracja w kodzie
                // services.Configure<MyOptions>(option => option.Port = 8899);

                // Pobieranie konfiguracji z pliku
                services.AddOptions();
                services.Configure<MyOptions>(hostContext.Configuration.GetSection("MyOptions"));

            });
    }
}