using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;

namespace dotnet_core_host_builder_demo
{
    public class MyOptions
    {
        public int Port { get; set; }
    }

    public class MyTcpServer : IHostedService
    {
        private readonly MyOptions options;
        private readonly ILogger logger;
        private readonly IRequestService requestService;

        public MyTcpServer(IOptions<MyOptions> options, ILogger<MyTcpServer> logger, IRequestService requestService)
        {
            this.options = options.Value;
            this.logger = logger;
            this.requestService = requestService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, options.Port);

            logger.LogInformation("Tcp listener started on port {port}", options.Port);

            listener.Start();
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                logger.LogInformation("a new client connected");

                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    {

                        while (client.Connected)
                        {
                            var request = await reader.ReadLineAsync();

                            if (request == null)
                            {
                                logger.LogInformation("Client disconnected.");
                                break;
                            }

                            logger.LogInformation("Received {request}", request);

                            string response = await requestService.GetResponse(request);

                            await writer.WriteLineAsync(response);

                            logger.LogInformation("Sent {response}", response);

                            writer.AutoFlush = true;
                        }
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Tcp listener stopped.");

            return Task.CompletedTask;
        }
    }

    
}
