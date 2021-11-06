using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Common.Helpers
{
    public class QueueMessageOptions
    {
        public TimeSpan? TTL { get; set; }
        public TimeSpan? Delay { get; set; }
    }
}
