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

        Boolean AddMessage<T>(T message);
        Boolean AddMessage(String message);
        Task<QueueProcessResult> ProcessMessages(Func<String, Int64, MessageProcessResult> processor);
    }
}
