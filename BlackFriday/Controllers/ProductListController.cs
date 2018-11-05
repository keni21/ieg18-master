using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlackFriday.Controllers
{
    [Route("api/getArray")]
    //http://blackfriday.azurewebsites.net/api/getArray
    public class ProductListController : Controller
    {
        private static readonly string productsArrayServiceBaseAddress = "https://productsarray20181105061241.azurewebsites.net/";

        // GET:  http://blackfriday.azurewebsites.net/api/getArray
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var response = GetMyResponse("/api/values/");

            return response.Content.ReadAsAsync<List<string>>().Result;
        }

        // http://blackfriday.azurewebsites.net/api/getArray/2
        [HttpGet("{id}")]
        public String Get(int id)
        {
            var response = GetMyResponse("/api/values/"+id);


            return response.Content.ReadAsStringAsync().Result;
        }

        private HttpResponseMessage GetMyResponse(String myRoute)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(productsArrayServiceBaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(productsArrayServiceBaseAddress + myRoute).Result;
            response.EnsureSuccessStatusCode();

            return response;
        }
    }

    [Route("api/getFile")]
    //https://productsfile20181105062602.azurewebsites.net/api/getFile
    public class ProductListFileController : Controller
    {
        private static readonly string fileArrayServiceBaseAddress = "https://productsfile20181105062602.azurewebsites.net";

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var response = GetMyResponse("/api/values/");

            return response.Content.ReadAsAsync<List<string>>().Result;
        }

        // http://blackfriday.azurewebsites.net/api/getFile/2
        [HttpGet("{id}")]
        public String Get(int id)
        {

            var response = GetMyResponse("/api/values/"+id);


            return response.Content.ReadAsStringAsync().Result;
        }

        private HttpResponseMessage GetMyResponse(String myRoute)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(fileArrayServiceBaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(fileArrayServiceBaseAddress + myRoute).Result;
            response.EnsureSuccessStatusCode();

            return response;
        }

    }
}
