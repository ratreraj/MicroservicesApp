using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StockMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMicroservice
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
            services.AddSingleton<IOrdersConsumer, OrdersConsumer>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockMicroservice", Version = "v1" });
            });

            services.AddMassTransit(config =>
            {
                config.AddConsumer<OrderConsumer>();
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(new Uri(Configuration["ServiceBus:Uri"]),
                    h =>
                    {
                        h.Username(Configuration["ServiceBus:Username"]);
                        h.Password(Configuration["ServiceBus:Password"]);
                    });
                    cfg.ReceiveEndpoint(Configuration["ServiceBus:Queue"], c =>
                    {
                        c.ConfigureConsumer<OrderConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockMicroservice v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var bus = app.ApplicationServices.GetService<IOrdersConsumer>();
            bus.RegisterOnMessageHandlerAndReceiveMessage();
        }
    }
}
