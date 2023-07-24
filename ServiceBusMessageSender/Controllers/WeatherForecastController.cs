using Azure.Messaging.ServiceBus;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMessageSender.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBusMessageSender.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ServiceBusClient _client;
        private readonly QueueClient _queueClient;
        private readonly DbConfig _dbConfig;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ServiceBusClient client, DbConfig dbConfig)
        {
            _logger = logger;
            _client = client;
            _dbConfig = dbConfig;
            _queueClient = new QueueClient(_dbConfig.AzureStorageConnectionString, _dbConfig.StorageQueueName);
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpPost]
        public async Task Post([FromBody] WeatherForecast data)
        {
            // send msg to queue
            //var sender = _client.CreateSender("post-weather-data");

            //var body = JsonConvert.SerializeObject(data);
            //var message = new ServiceBusMessage(body);

            //await sender.SendMessageAsync(message);



            // send msg to topic 
            //var sender = _client.CreateSender("forcast-topic");

            //var body = JsonConvert.SerializeObject(data);
            //var message = new ServiceBusMessage(body);

            //await sender.SendMessageAsync(message);


            // send msg to azure storage queue

            await _queueClient.SendMessageAsync(JsonConvert.SerializeObject(data), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));

            ////msg never expires
            //await _queueClient.SendMessageAsync(JsonConvert.SerializeObject(data), null, TimeSpan.FromSeconds(-1));


        }
    }
}
