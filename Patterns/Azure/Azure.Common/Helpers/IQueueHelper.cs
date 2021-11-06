using System;

namespace Azure.Common.Helpers
{
    public interface IQueueHelper<MessageClass>
    {
        Boolean AddMessage<T>(T message);
        Boolean AddMessage(String message);        
        Boolean ProcessMessages(Func<MessageClass, Boolean> processor);
    }
}
