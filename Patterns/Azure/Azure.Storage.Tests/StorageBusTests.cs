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
            String connectionString = fixture.Configuration.StorageConnectionString;
            String queueName = "testqueue";
            IQueueHelper<QueueMessage> queueHelper = new StorageQueueHelper(nullLogger, connectionString, queueName);

            // ACT
            Boolean result = queueHelper.AddMessage("");

            // ASSERT
            result.Should().BeTrue();
        }
    }
}
