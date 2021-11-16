using Azure.Common.Helpers;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
using FluentAssertions;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using TNDStudios.TestHelpers;
using Moq;

namespace Azure.Storage.Tests
{
    public class StorageBusTestConfiguration
    {
        public string StorageConnectionString { get; set; } = String.Empty;
    }

    public class StorageBusTestFixture : IDisposable
    {
        public StorageBusTestConfiguration Configuration { get; set; }

        public StorageBusTestFixture()
        {
            Configuration = ConfigurationHelper.GetConfiguration<StorageBusTestConfiguration>(String.Empty);
        }

        public void Dispose()
        {
            // clean up test data from the database
        }
    }

    public class StorageBusTests : IClassFixture<StorageBusTestFixture>
    {
        StorageBusTestFixture fixture;

        public StorageBusTests(StorageBusTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Adding_Message_To_Bus_Should_Result_In_True()
        {
            // ARRANGE
            ILogger nullLogger = new Mock<ILogger>().Object;
            String messageContext = "This is a test";
            String connectionString = fixture.Configuration.StorageConnectionString;
            String queueName = "addmsgtest";
            IQueueHelper<QueueMessage> queueHelper = new StorageQueueHelper(nullLogger, connectionString, queueName, new QueueMessageOptions { });
            Func<String, Int64, MessageProcessResult> processor = (String content, Int64 deliveryCount) =>
                                                       {
                                                           return new MessageProcessResult()
                                                           {
                                                               Success = (content == messageContext)
                                                           };
                                                       };

            // ACT
            queueHelper.Destroy(); // Kill an existing queue if there is one
            queueHelper.Create(); // Create a fresh queue
            Boolean addResult = queueHelper.AddMessage(messageContext);
            QueueProcessResult result = queueHelper.ProcessMessages(processor).Result;
            Int32 endLength = queueHelper.Length;

            // ASSERT
            addResult.Should().BeTrue();
            result.Success.Should().Be(1);
            result.Fail.Should().Be(0);
            endLength.Should().Be(0);
        }

        [Fact]
        public void Failing_Message_On_Bus_Should_Result_In_Defined_Delay()
        {
            // ARRANGE
            Int32 secondsRequeueDelay = 5;
            ILogger nullLogger = new Mock<ILogger>().Object;
            String messageContext = "This is a test";
            String connectionString = fixture.Configuration.StorageConnectionString;
            String queueName = "addmsgtest";
            TimeSpan delayLower = new TimeSpan(0, 0, secondsRequeueDelay);
            TimeSpan delayUpper = new TimeSpan(0, 0, secondsRequeueDelay * 2);
            IQueueHelper<QueueMessage> queueHelper = new StorageQueueHelper(nullLogger, connectionString, queueName, new QueueMessageOptions { Delay = delayLower });
            Func<String, Int64, MessageProcessResult> processor = (String content, Int64 deliveryCount) =>
            {
                return new MessageProcessResult()
                {
                    Success = (deliveryCount > 1) && (content == messageContext)
                };
            };

            // ACT
            queueHelper.Destroy(); // Kill an existing queue if there is one
            queueHelper.Create(); // Create a fresh queue
            Boolean addResult = queueHelper.AddMessage(messageContext);
            QueueProcessResult result = null;
            DateTime start = DateTime.UtcNow;
            while (result == null || (result.Success == 0))
            {
                result = queueHelper.ProcessMessages(processor).Result;
            }
            DateTime finished = DateTime.UtcNow;
            TimeSpan processingTime = finished - start;

            Int32 endLength = queueHelper.Length;

            // ASSERT
            addResult.Should().BeTrue();
            result.Success.Should().Be(1);
            result.Fail.Should().Be(0);
            endLength.Should().Be(0);
            processingTime.Should().BeGreaterThanOrEqualTo(delayLower);
            processingTime.Should().BeLessThanOrEqualTo(delayUpper);
        }
    }
}
