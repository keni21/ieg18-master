using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Polly.CircuitBreaker;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace BlackFriday.Controllers
{
    [Produces("application/json")]
    [Route("api/PaymentMethods")]
    public class PaymentMethodsController : Controller
    {
        //https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        private readonly ILogger<PaymentMethodsController> _logger;
        private readonly IHttpClientFactory _clientFactory;


        public PaymentMethodsController(ILogger<PaymentMethodsController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            List<string> acceptedPaymentMethods = null;
            _logger.LogError("Accepted Paymentmethods");

          

            var client = _clientFactory.CreateClient("https://iegeasycreditcardservice20181105063259.azurewebsites.net/");


            //            client.BaseAddress = new Uri(creditcardServiceBaseAddress);
            //    client.DefaultRequestHeaders.Accept.Clear();
            //      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await client.GetAsync("api/AcceptedCreditCards");
            }
            catch (BrokenCircuitException exception)
            {
                response = await failedCircuit(exception);
            }
            if (response.IsSuccessStatusCode)
            {
                acceptedPaymentMethods = response.Content.ReadAsAsync<List<string>>().Result;
            }
            return acceptedPaymentMethods;
        }

        private async Task<HttpResponseMessage> failedCircuit(BrokenCircuitException exception) {
            _logger.LogError(" Trying second Payment " + exception.Message);
            var client = _clientFactory.CreateClient("https://iegeasycreditcardservice-2.azurewebsites.net/");
            HttpResponseMessage response = await client.GetAsync("api/AcceptedCreditCards");
            return response;
        }

        private void logger(List<string> acceptedPayment) {
            foreach (var item in acceptedPayment) {
                _logger.LogError("Payment " + item);
            }
        }
    }
}