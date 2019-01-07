using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataReceiver.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Consul;

namespace DataReceiver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            _createConClients(services, "02");
            _createConClients(services, "22");
            _registerService();
        }

        private void _createClient(IServiceCollection services, int v)
        {
            throw new NotImplementedException();
        }

        private void _createConClients(IServiceCollection iServices, String conNumber)
        {
            // String conString = "http://easycreditcardservice" + conNumber + ".azurewebsites.net/";
            String conString = "http://"+_Url(conNumber) + "/";

            iServices.AddHttpClient(conString, client =>
            {
                client.BaseAddress = new Uri(conString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            })
            .AddTransientHttpErrorPolicy(builder => retryDo())
            .AddTransientHttpErrorPolicy(builder => breakCircuitDo());
        }

        private IAsyncPolicy<HttpResponseMessage> retryDo()
        {
            return HttpPolicyExtensions.HandleTransientHttpError().RetryAsync(1);
        }

        private IAsyncPolicy<HttpResponseMessage> breakCircuitDo()
        {
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(2, TimeSpan.FromSeconds(69));
        }

        private String _Url(String conNumber)
        {
            HttpClient ConsuleClient = new HttpClient();
            ConsuleClient.BaseAddress = new Uri("http://127.0.0.1");
            ConsuleClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage ConsuleResponse = ConsuleClient.GetAsync("http://127.0.0.1:5010/api/info/handler50"+ conNumber).Result;
            var test = ConsuleResponse.Content.ReadAsStringAsync().Result;
            return ConsuleResponse.Content.ReadAsStringAsync().Result;
        }

        private async void _registerService() {
            JObject myService = new JObject();
            myService.Add("port", 5001);
            myService.Add("name", "receiver");
            myService.Add("address", "127.0.0.1");
            myService.Add("uri", "127.0.0.1:5001");
            myService.Add("id", "receiver5001");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync("http://127.0.0.1:5010/api/register", myService);
            response.EnsureSuccessStatusCode();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
