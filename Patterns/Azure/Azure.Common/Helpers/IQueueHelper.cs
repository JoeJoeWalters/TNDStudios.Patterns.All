using System;
using System.Threading.Tasks;

namespace Azure.Common.Helpers
{
    public interface IQueueHelper<MessageClass>
    {
        Boolean IsAvailable { get; }
        Int32 Length { get; }

        Boolean Create();
        Boolean Destroy();

        Boolean AddMessage<T>(T message, QueueMessageOptions options = null);
        Boolean AddMessage(String message, QueueMessageOptions options = null);
        Task<Boolean> ProcessMessages(Func<String, Boolean> processor);
    }
}
