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
            Func<String, Int64,  Boolean> processor = (String content, Int64 deliveryCount) => 
                                                        { 
                                                            return content == messageContext; 
                                                        };

            // ACT
            queueHelper.Destroy(); // Kill an existing queue if there is one
            queueHelper.Create(); // Create a fresh queue
            Boolean addResult = queueHelper.AddMessage(messageContext);
            Boolean fetchResult = queueHelper.ProcessMessages(processor).Result;
            Int32 endLength = queueHelper.Length;

            // ASSERT
            addResult.Should().BeTrue();
            fetchResult.Should().BeTrue();
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
            IQueueHelper<QueueMessage> queueHelper = new StorageQueueHelper(nullLogger, connectionString, queueName, new QueueMessageOptions { });
            Func<String, Int64, Boolean> processor = (String content, Int64 deliveryCount) =>
            {
                return (deliveryCount > 1) && (content == messageContext);
            };

            // ACT
            queueHelper.Destroy(); // Kill an existing queue if there is one
            queueHelper.Create(); // Create a fresh queue
            Boolean addResult = queueHelper.AddMessage(messageContext);
            Boolean fetchResult = false;
            DateTime start = DateTime.UtcNow;
            while (!fetchResult)
            {
                fetchResult = queueHelper.ProcessMessages(processor).Result;
            }
            DateTime finished = DateTime.UtcNow;

            Int32 endLength = queueHelper.Length;

            // ASSERT
            addResult.Should().BeTrue();
            fetchResult.Should().BeTrue();
            endLength.Should().Be(0);
        }
    }
}
