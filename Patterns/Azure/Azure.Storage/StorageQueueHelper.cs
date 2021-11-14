using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Common.Helpers;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Azure.Storage
{
    public static class QueueMessageExtensions
    {
        public static String BodyAsString(this QueueMessage value)
            => Encoding.UTF8.GetString(value.Body.ToArray());
    }

    public class StorageQueueHelper : IQueueHelper<QueueMessage>
    {
        private readonly QueueClient _QueueClient;
        private readonly ILogger _Logger;
        private readonly QueueMessageOptions _Options;

        public Boolean IsAvailable
        {
            get
            {
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
            get
            {
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

        public StorageQueueHelper(ILogger logger, String connectionString, String queueName, QueueMessageOptions options)
        {
            _Logger = logger;
            _Options = options ?? new QueueMessageOptions()
            {
                TTL = null,
                Delay = null
            };
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

        public Boolean AddMessage<T>(T message)
            => AddMessage(JsonConvert.SerializeObject(message));

        public Boolean AddMessage(String message)
        {
            if (IsAvailable)
            {
                try
                {
                    _QueueClient.SendMessage(message, _Options.Delay, _Options.TTL);
                    _Logger.LogInformation($"Added message to queue - {_QueueClient.Name}");
                    return true;
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Failed to add message to queue - {_QueueClient.Name}", ex);
                }
            }

            return false;
        }

        public async Task<QueueProcessResult> ProcessMessages(Func<String, Int64, MessageProcessResult> processor)
        {
            QueueProcessResult result = new QueueProcessResult();
            if (IsAvailable)
            {
                try
                {
                    QueueMessage[] messages = await _QueueClient.ReceiveMessagesAsync(visibilityTimeout: _Options.Delay);
                    foreach (QueueMessage message in messages)
                    {
                        String content = message.BodyAsString();
                        MessageProcessResult processResult = processor(content, message.DequeueCount);
                        if (processResult.Success)
                        {
                            // Delete the processed message to avoid the lease expiring and it being visible again to another process
                            await _QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                            result.Success++;
                        }
                        else
                        {
                            result.Fail++;

                            // Has the processor logic overridden the message delay with something non-default?
                            if (processResult.Delay.HasValue)
                            {
                                _QueueClient.UpdateMessage(message.MessageId, message.PopReceipt, visibilityTimeout : processResult.Delay.Value);
                            }
                        }
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
