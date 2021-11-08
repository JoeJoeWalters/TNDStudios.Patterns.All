using System;

namespace Azure.Common.Helpers
{
    public interface IQueueHelper<MessageClass>
    {
        Boolean IsAvailable { get; }
        Int32 Length { get; }

        Boolean AddMessage<T>(T message, QueueMessageOptions options = null);
        Boolean AddMessage(String message, QueueMessageOptions options = null);       
        Boolean ProcessMessages(Func<MessageClass, Boolean> processor);
    }
}
