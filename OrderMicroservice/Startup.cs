using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderMicroservice
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
            services.AddHttpClient("basketmicroservice")
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                   .AddPolicyHandler(GetRetryPolicy())
                   .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderMicroservice", Version = "v1" });
            });
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(Configuration["ServiceBus:Uri"]),
                    h =>
                    {
                        h.Username(Configuration["ServiceBus:Username"]);
                        h.Password(Configuration["ServiceBus:Password"]);
                    });
                });
            });
            services.AddMassTransitHostedService();

            //opentelemetry
            services.AddOpenTelemetryTracing((builder) => builder
                      .AddAspNetCoreInstrumentation()
                      .AddHttpClientInstrumentation()
                      .AddZipkinExporter());
            services.Configure<ZipkinExporterOptions>(Configuration.GetSection("Zipkin"));
        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
             .HandleTransientHttpError()
             .CircuitBreakerAsync(2, TimeSpan.FromSeconds(10)); //after 2 times failure, it will open circuit
        }

        //retry pattern
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // HttpRequestException, 5XX and 408 --timeout 
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound) // 404  
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(4));  // Retry two times after 4 sec delay 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderMicroservice v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
