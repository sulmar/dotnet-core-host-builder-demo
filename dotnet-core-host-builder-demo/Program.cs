using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace dotnet_core_host_builder_demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // add package Microsoft.Extensions.Hosting
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IHostedService, MyHostedService>();
                    services.AddScoped<IService, MyService>();

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

    public interface IService
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }


    public class MyOptions
    {
        public int Port { get; set; }
    }

    public class MyService : IService
    {
        private readonly MyOptions options;
        private readonly ILogger logger;


        public MyService(IOptions<MyOptions> options, ILogger<MyService> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, options.Port);

            logger.LogInformation("Tcp listener started on port {port}", options.Port);

            listener.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                logger.LogInformation("a new client connected");

                using (NetworkStream stream = client.GetStream())
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        byte[] data = new byte[1024];

                        int read = await stream.ReadAsync(data, 0, 1024, cancellationToken);

                        var cmd = Encoding.UTF8.GetString(data, 0, read);
                        logger.LogInformation("received {cmd}", cmd);

                        string response = "OK";

                        var responseBytes = Encoding.UTF8.GetBytes(response);

                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length, cancellationToken);
                        stream.Flush();
                    }
                }
            }



            }
    }

    public class MyHostedService : IHostedService
    {
        private readonly IService service;

        public MyHostedService(IService service)
        {
            this.service = service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MyHostedService Started!");

            service.ExecuteAsync(cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MyHostedService Stopped!");

            return Task.CompletedTask;
        }
    }
}
