using Azure.Storage.Queues;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBusMessageSender.models;
using ServiceBusMessageSender.Services;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ServiceBusMessageSender
{
    public class Startup
    {
        private readonly DbConfig _dbConfig;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _dbConfig = new DbConfig();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Configuration.GetSection("Database").Bind(_dbConfig);
            services.AddSingleton(_dbConfig);
            services.AddControllers();

            services.AddSwaggerGen();
           services.AddHostedService<WeatherDataService>();

            // add the service bus dependecy
            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClient(_dbConfig.ServiceBusConnectionString);
            });

            //services.AddAzureClients(builder =>
            //{
            //    builder.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
            //    {
            //        return new QueueClient(_dbConfig.AzureStorageConnectionString, _dbConfig.StorageQueueName);
            //    });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.ConfigObject = new ConfigObject
                {
                    ShowCommonExtensions = true
                };

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForcast API");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
