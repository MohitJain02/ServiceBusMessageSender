using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SendEmailTopicListener
{
    public class TopicListener
    {
        private readonly ILogger<TopicListener> _logger;

        public TopicListener(ILogger<TopicListener> log)
        {
            _logger = log;
        }

        [FunctionName("TopicListener")]
        public void Run([ServiceBusTrigger("forcast-topic", "send-email", Connection = "ServiceBusConnectionString")]string mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
