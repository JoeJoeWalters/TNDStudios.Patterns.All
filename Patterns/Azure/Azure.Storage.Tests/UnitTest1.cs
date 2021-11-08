using Azure.Common.Helpers;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
using FluentAssertions;

namespace Azure.Storage.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Adding_Message_To_Bus_Should_Result_In_True()
        {
            // ARRANGE
            ILogger nullLogger = null;
            String connectionString = String.Empty;
            String queueName = String.Empty;
            IQueueHelper<QueueMessage> queueHelper = new StorageQueueHelper(nullLogger, connectionString, queueName);

            // ACT
            Boolean result = queueHelper.AddMessage("");

            // ASSERT
            result.Should().BeTrue();
        }
    }
}
