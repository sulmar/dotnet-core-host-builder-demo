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


    public static IHostBuilder UseTcpService(this IHostBuilder hostBuilder, Func<string, string> request)
    {
        return hostBuilder.UseHostedService<TcpHostedServer>()
            .ConfigureServices((hostContext, services) =>
            {
               services.AddSingleton<Func<string, string>>(request);

                // Get default options
                 services.Configure<TcpHostedServerOptions>(option => option.Port = 5000);

                // Get options from configuration
                services.AddOptions();
                services.Configure<TcpHostedServerOptions>(hostContext.Configuration.GetSection("TcpHostedServer"));

            });
    }

  
}