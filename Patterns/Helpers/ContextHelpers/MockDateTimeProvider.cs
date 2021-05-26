using System;
using System.Collections.Generic;
using System.Text;

namespace TNDStudios.ContextHelpers
{
    public class MockDateTimeProvider : IDateTimeProvider
    {
        private DateTime _current;

        public MockDateTimeProvider() { _current = new DateTime(); }
        public MockDateTimeProvider(DateTime current) { _current = current; }

        public DateTime Get() => _current;
     
        public void Set(DateTime value) => _current = value;
    }
}
