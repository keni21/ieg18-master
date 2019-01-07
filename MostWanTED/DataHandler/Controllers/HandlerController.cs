using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DataHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandlerController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public HandlerController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "handler1", "handler2" };
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
        public async Task<JObject> ReceiveSurveyValues(JObject surveyValues)
        {
            Double participants = (double)surveyValues["val1"] + (double)surveyValues["val2"] + (double)surveyValues["val3"] + (double)surveyValues["val3"];
            Double percent1 = participants > 0 ? (double)surveyValues["val1"] * 100 / participants : 0;
            Double percent2 = participants > 0 ? (double)surveyValues["val2"] * 100 / participants : 0;
            Double percent3 = participants > 0 ? (double)surveyValues["val3"] * 100 / participants : 0;
            Double percent4 = participants > 0 ? (double)surveyValues["val4"] * 100 / participants : 0;

            JObject results = new JObject();
            results.Add("name", (String)surveyValues["name"]);
            results.Add("minAge", (int)surveyValues["minAge"]);
            results.Add("maxAge", (int)surveyValues["maxAge"]);
            results.Add("participants", participants);
            results.Add((String)surveyValues["type1"], percent1);
            results.Add((String)surveyValues["type2"], percent2);
            results.Add((String)surveyValues["type3"], percent3);
            results.Add((String)surveyValues["type4"], percent4);
            var client = _createClient();
            HttpResponseMessage response = await client.PostAsJsonAsync("api/sender", results);
            return response.Content.ReadAsAsync<JObject>().Result;
        }

        private HttpClient _createClient()
        {
            HttpClient ConsuleClient = new HttpClient();
            ConsuleClient.BaseAddress = new Uri("http://127.0.0.1");
            ConsuleClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage ConsuleResponse = ConsuleClient.GetAsync("http://127.0.0.1:5010/api/info/sender5003").Result;
            var test = ConsuleResponse.Content.ReadAsStringAsync().Result;
            HttpClient client = _clientFactory.CreateClient("http://" + ConsuleResponse.Content.ReadAsStringAsync().Result + "/");
            return client;
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
