using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;
using DiscoveryService.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using DiscoveryService.Services;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace DiscoveryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private Func<IConsulClient> _consulClientFactory;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<InfoController> _logger;

        public IConfiguration Configuration { get; }

        public InfoController(Func<IConsulClient> consulClientFactory, IConfiguration configuration, IHttpClientFactory clientFactory,
                             ILogger<InfoController> logger)
        {
            _consulClientFactory = consulClientFactory;
            Configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(String id)
        {
            String _serverUrl = "";
            var consuleClient = new ConsulClient(c => c.Address = new Uri("http://127.0.0.1:8500"));
            var services = consuleClient.Agent.Services().Result.Response;
            foreach (var service in services)
            {
                if (service.Value.ID == id)
                {
                    _serverUrl = service.Value.Address + ":" + service.Value.Port;
                }
            }
            return _serverUrl;
        }

    }
}
