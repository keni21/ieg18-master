﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataReceiver.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using Polly.CircuitBreaker;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Extensions;

namespace DataReceiver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiverController : ControllerBase
    {
       // private readonly ILogger<PaymentMethodsController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private object features;

        public ReceiverController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "receiver1", "receiver2" };
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

        [HttpPost]
        public async Task<JObject> ReceiveSurveyValues([FromBody] ReceiverModel surveyValues)
        {
            JObject handlerResponse = null;
            HttpClient client = _createClient("02");

            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await client.PostAsJsonAsync("api/handler", surveyValues);
            }
            catch (BrokenCircuitException exception)
            {
                response = await failedCircuit(exception, surveyValues);
            }
            if (response.IsSuccessStatusCode)
            {
                handlerResponse = response.Content.ReadAsAsync<JObject>().Result;
            }
            return handlerResponse;
        }

        private async Task<HttpResponseMessage> failedCircuit(BrokenCircuitException exception, ReceiverModel surveyValues)
        {
            var client = _createClient("22");
            HttpResponseMessage response = await client.PostAsJsonAsync("api/handler", surveyValues);
            return response;
        }

        private  HttpClient _createClient(String conNumber) {
            HttpClient ConsuleClient = new HttpClient();
            ConsuleClient.BaseAddress = new Uri("http://127.0.0.1");
            ConsuleClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage ConsuleResponse =  ConsuleClient.GetAsync("http://127.0.0.1:5010/api/info/handler50"+ conNumber).Result;
            HttpClient client = _clientFactory.CreateClient("http://" + ConsuleResponse.Content.ReadAsStringAsync().Result + "/");
            return client;
        }
    }
}

