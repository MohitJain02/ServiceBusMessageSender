namespace ServiceBusMessageSender.models
{
    public class DbConfig
    {
        public string ServiceBusConnectionString { get; set; }

        public string AzureStorageConnectionString { get; set; }

        public string StorageQueueName { get; set; }

    }
}
