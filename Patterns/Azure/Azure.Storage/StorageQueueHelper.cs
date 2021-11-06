using System;
using Azure.Common.Helpers;
using Azure.Storage.Queues; 
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.Storage
{
    public class StorageQueueHelper : IQueueHelper<QueueMessage>
    {
        private readonly QueueClient _QueueClient;
        private readonly ILogger _Logger;

        public Boolean IsAvailable
        { 
            get {
                try
                {
                    return _QueueClient.Exists();
                }
                catch
                {
                    return false;
                }
            }
        }

        public StorageQueueHelper(ILogger logger, String connectionString, String queueName)
        {
            _Logger = logger;
            _QueueClient = new QueueClient(connectionString, queueName);
            _QueueClient.CreateIfNotExists();
        }

        public Boolean AddMessage<T>(T message)
            => AddMessage(JsonConvert.SerializeObject(message));

        public Boolean AddMessage(String message)
        {
            if (IsAvailable)
            {
                try
                {
                    _QueueClient.SendMessage(message);
                    _Logger.LogInformation($"Added message to queue - {_QueueClient.Name}");
                    return true;
                }
                catch(Exception ex)
                {
                    _Logger.LogError($"Failed to add message to queue - {_QueueClient.Name}", ex);
                }
            }

            return false;
        }

        public Boolean ProcessMessages(Func<QueueMessage, Boolean> processor)
        {
            Boolean result = true;
            if (IsAvailable)
            {
                try
                {
                    QueueMessage[] messages = _QueueClient.ReceiveMessages();
                    foreach (QueueMessage message in messages)
                    {
                        if (!processor(message))
                            result = false;
                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Failed to add message to queue - {_QueueClient.Name}", ex);
                }
            }

            return result;
        }
    }
}
