using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BlackFriday.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Rest.TransientFaultHandling;
using Swashbuckle.AspNetCore.Swagger;
using System.Web.Http;
using Microsoft.AspNet.WebHooks;

namespace BlackFriday
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
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BlackFriday API", Version = "v1" });
            });

            _createConClients(services, "20181105063259");
            _createConClients(services, "-2");
            services.AddApplicationInsightsTelemetry(Configuration);
        }

        private void _createClient(IServiceCollection services, int v)
        {
            throw new NotImplementedException();
        }

        private void _createConClients(IServiceCollection iServices, String conNumber)
        {
            // String conString = "http://easycreditcardservice" + conNumber + ".azurewebsites.net/";
            String conString = "https://iegeasycreditcardservice"+conNumber+".azurewebsites.net/";



            iServices.AddHttpClient(conString, client =>
            {
                client.BaseAddress = new Uri(conString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            })
            .AddTransientHttpErrorPolicy(builder => retryDo())
            .AddTransientHttpErrorPolicy(builder => breakCircuitDo());
        }

        private IAsyncPolicy<HttpResponseMessage> retryDo () {
            return HttpPolicyExtensions.HandleTransientHttpError().RetryAsync(1);
        }

        private IAsyncPolicy<HttpResponseMessage> breakCircuitDo () {
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(2, TimeSpan.FromSeconds(69));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Black Friday API");
            });
            app.UseMvc();
        }
    }
}
