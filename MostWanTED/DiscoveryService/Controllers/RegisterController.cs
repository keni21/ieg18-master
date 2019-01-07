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

namespace DiscoveryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private Func<IConsulClient> _consulClientFactory;
        private readonly IHttpClientFactory _clientFactory;

        public IConfiguration Configuration { get; }

        public RegisterController(Func<IConsulClient> consulClientFactory, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _consulClientFactory = consulClientFactory;
            Configuration = configuration;
            _clientFactory = clientFactory;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "register1", "register2" };
        }


        [HttpPost]
        public async void RegisterService(JObject NewService)
        {
            var client = new ConsulClient(); // uses default host:port which is localhost:8500
            AgentServiceCheck httpCheck = _httpCheck((String)NewService["uri"], (String)NewService["name"]);
            var ServiceReg = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                Address = (String)NewService["address"],
                ID = (String)NewService["id"],
                Name = (String)NewService["name"] + ":" + NewService["port"],
                Port = (int)NewService["port"]
            };

            await client.Agent.ServiceRegister(ServiceReg);           
        }
       
        private AgentServiceCheck _httpCheck(String uri, String type)
        {
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = "http://" + uri + "/api/"+type+"/status"
            };
            return httpCheck;
        }
    }
}
