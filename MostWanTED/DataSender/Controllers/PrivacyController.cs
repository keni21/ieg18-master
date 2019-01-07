using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DataSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivacyController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public PrivacyController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "sender1", "sender2" };
        }

        // GET api/values
        [HttpGet("status")]
        public IActionResult Status() => Ok();

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<String> GetToken([FromBody] JObject login)
        {
            var client = _createClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("api/verify", login);
            JObject token = response.Content.ReadAsAsync<JObject>().Result;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (String)token["token"]);
            HttpResponseMessage SecrectResponse = await client.GetAsync("api/verify");
            var test = SecrectResponse.Content.ReadAsStringAsync().Result;
            return SecrectResponse.Content.ReadAsStringAsync().Result;
        }

        private HttpClient _createClient()
        {
            HttpClient ConsuleClient = new HttpClient();
            ConsuleClient.BaseAddress = new Uri("http://127.0.0.1");
            ConsuleClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage ConsuleResponse = ConsuleClient.GetAsync("http://127.0.0.1:5010/api/info/secrets5007").Result;
            var test = ConsuleResponse.Content.ReadAsStringAsync().Result;
            HttpClient client = _clientFactory.CreateClient("http://" + ConsuleResponse.Content.ReadAsStringAsync().Result + "/");
            return client;
        }
    }
}
