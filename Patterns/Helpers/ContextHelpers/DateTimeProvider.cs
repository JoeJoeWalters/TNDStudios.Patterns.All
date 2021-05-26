using System;
using System.Collections.Generic;
using System.Text;

namespace TNDStudios.ContextHelpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeProvider() {}

        public DateTime Get() => DateTime.Now;
     
        public void Set(DateTime value) => throw new NotImplementedException();
    }
}
