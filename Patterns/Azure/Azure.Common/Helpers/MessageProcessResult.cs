using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Common.Helpers
{
    public class MessageProcessResult
    {
        public Boolean Success { get; set; } = false;
        public TimeSpan? Delay { get; set; } = null;
    }
}
