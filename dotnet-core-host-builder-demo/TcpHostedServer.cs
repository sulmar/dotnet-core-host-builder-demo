using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System;

namespace dotnet_core_host_builder_demo
{

    public class TcpHostedServer : IHostedService, IDisposable
    {
        private readonly TcpHostedServerOptions options;
        private readonly ILogger logger;

        private Func<string, string> func;

        public TcpHostedServer(
            IOptions<TcpHostedServerOptions> options, 
            ILogger<TcpHostedServer> logger = null, 
            Func<string, string> func = null)
        {
            this.options = options.Value;
            this.logger = logger;
            this.func = func;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, options.Port);

            logger?.LogInformation("Tcp listener started on port {port}", options.Port);

            listener.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                logger?.LogInformation("a new client connected");

                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    while (client.Connected)
                    {
                        var request = await reader.ReadLineAsync();

                        if (request == null)
                        {
                            logger?.LogInformation("Client disconnected.");
                            break;
                        }

                        logger?.LogInformation("Received {request}", request);

                        string response = func?.Invoke(request);
                       
                        await writer.WriteLineAsync(response);

                        logger?.LogInformation("Sent {response}", response);

                        writer.AutoFlush = true;
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger?.LogInformation("Tcp listener stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

    }


}
