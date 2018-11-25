using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DataHandler
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
            _registerService();
            _createConClients(services, 5003);
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

        private async void _registerService()
        {
            JObject myService = new JObject();
            myService.Add("port", 5002);
            myService.Add("name", "handler");
            myService.Add("address", "127.0.0.1");
            myService.Add("uri", "127.0.0.1:5002");
            myService.Add("id", "handler5002");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync("http://127.0.0.1:5010/api/register", myService);
            response.EnsureSuccessStatusCode();
        }

        private void _createConClients(IServiceCollection iServices, int conNumber)
        {
            // String conString = "http://easycreditcardservice" + conNumber + ".azurewebsites.net/";
            String conString = "http://127.0.0.1:" + conNumber + "/";


            iServices.AddHttpClient(conString, client =>
            {
                client.BaseAddress = new Uri(conString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            });
        }
    }
}
