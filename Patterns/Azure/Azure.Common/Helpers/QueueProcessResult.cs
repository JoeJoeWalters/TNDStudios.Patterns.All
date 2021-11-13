using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Common.Helpers
{
    public class QueueProcessResult
    {
        public Int32 Success { get; set; } = 0;
        public Int32 Fail { get; set; } = 0;
    }
}
