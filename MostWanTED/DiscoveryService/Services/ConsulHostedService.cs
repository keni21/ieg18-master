using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DiscoveryService.Config;

namespace DiscoveryService.Services
{
    public class ConsulHostedService : IHostedService
    {
        private CancellationTokenSource _cts;
        private readonly IConsulClient _consulClient;
        private readonly IOptions<ConsulConfig> _consulConfig;
        private readonly ILogger<ConsulHostedService> _logger;
        private readonly IServer _server;
        private string _registrationID;

        public AgentServiceRegistration registerNewService { get; set; }

        public ConsulHostedService(
            IConsulClient consulClient,
            IOptions<ConsulConfig> consulConfig,
            ILogger<ConsulHostedService> logger,
            IServer server)
        {
            _server = server;
            _logger = logger;
            _consulConfig = consulConfig;
            _consulClient = consulClient;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            //await registerHelper("http://127.0.0.1", 5001, "/api/receiver/status", "Receiver");
            //await registerHelper("http://127.0.0.1", 5002, "/api/handler/status", "Handler");
            //await registerHelper("http://127.0.0.1", 5022, "/api/handler/status", "Handler");
            //await registerHelper("http://127.0.0.1", 5003, "/api/sender/status", "Sender");

        }

        private async Task registerHelper(String uri, int port, String prefix, String name) {
            AgentServiceRegistration ServiceRegister = new AgentServiceRegistration()
            {
                ID = "ServicePort-" + port,
                Name = name + port,
                Address = uri,
                Port = port,
                Check = new AgentServiceCheck()
                {
                    HTTP = uri + ":" + port + prefix,
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            _logger.LogInformation("Registering in Consul");
            await _consulClient.Agent.ServiceDeregister(ServiceRegister.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(ServiceRegister, _cts.Token);
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.LogInformation("Deregistering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationID, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Deregisteration failed");
            }
        }
    }
}
