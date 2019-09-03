﻿using Microsoft.Extensions.Hosting;
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
    public class MyOptions
    {
        public int Port { get; set; }
    }

    public delegate Task<string> RequestTcp(string content);

    public class MyTcpServer : IHostedService, IDisposable
    {
        private readonly MyOptions options;
        private readonly ILogger logger;
        private readonly RequestTcp execute;

        public MyTcpServer(IOptions<MyOptions> options, ILogger<MyTcpServer> logger = null, RequestTcp execute = null)
        {
            this.options = options.Value;
            this.logger = logger;
            this.execute = execute;
        }

      

        public void Dispose()
        {
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

                        string response = await execute?.Invoke(request);

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
    }

    
}
