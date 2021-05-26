using System;

namespace TNDStudios.ContextHelpers
{
    public interface IDateTimeProvider
    {
        DateTime Get();
        void Set(DateTime value);
    }
}
