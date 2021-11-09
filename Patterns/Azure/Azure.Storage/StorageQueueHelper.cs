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

        public Int32 Length 
        {
            get { 
                try
                {
                    QueueProperties properties = _QueueClient.GetProperties();
                    return properties.ApproximateMessagesCount;
                }
                catch 
                {
                    return 0;
                }
            } 
        }

        public StorageQueueHelper(ILogger logger, String connectionString, String queueName)
        {
            _Logger = logger;
            _QueueClient = new QueueClient(connectionString, queueName);
        }

        public Boolean Create()
        {
            try
            {
                _QueueClient.CreateIfNotExists();
            }
            catch 
            {
                return false;
            }

            return true;
        }

        public Boolean Destroy()
        {
            try
            {
                _QueueClient.DeleteIfExists();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Boolean AddMessage<T>(T message, QueueMessageOptions options = null)
            => AddMessage(JsonConvert.SerializeObject(message), null);

        public Boolean AddMessage(String message, QueueMessageOptions options = null)
        {
            if (IsAvailable)
            {
                options = options ?? new QueueMessageOptions()
                                            {
                                                TTL = null,
                                                Delay = null
                                            };

                try
                {
                    _QueueClient.SendMessage(message, options.Delay, options.TTL);
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
