using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusMessageSender.models;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusMessageSender.Services
{
    public class WeatherDataService : BackgroundService
    {
        private readonly ILogger<WeatherDataService> _logger;
        private readonly QueueClient _queueClient;
        private readonly DbConfig _dbConfig;
        public WeatherDataService(ILogger<WeatherDataService> logger, DbConfig dbConfig)
        {
            _logger = logger;  
            _dbConfig = dbConfig;
            _queueClient = new QueueClient(_dbConfig.AzureStorageConnectionString, _dbConfig.StorageQueueName);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Reading From Queue");
                var queueMessage = await _queueClient.ReceiveMessageAsync();

                if(queueMessage.Value != null)
                {
                    var weatherData = JsonConvert.DeserializeObject<WeatherForecast>(queueMessage.Value.MessageText);

                    _logger.LogInformation($"New message Read:{weatherData.Summary}", weatherData);

                    await _queueClient.DeleteMessageAsync(queueMessage.Value.MessageId, queueMessage.Value.PopReceipt);
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
