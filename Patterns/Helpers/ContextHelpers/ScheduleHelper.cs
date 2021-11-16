using NCrontab;
using System;
using System.Collections.Generic;

namespace TNDStudios.ContextHelpers
{
    public class ScheduleHelper
    {
        private readonly string _cron;

        public ScheduleHelper(string cron)
        {
            _cron = cron;
        }

        public DateTime GetNext()
        {
            var schedule = CrontabSchedule.Parse(_cron);
            return schedule.GetNextOccurrence(DateTime.Now);
        }

        public IEnumerable<DateTime> GetNextX(DateTime end)
        {
            var schedule = CrontabSchedule.Parse(_cron);
            return schedule.GetNextOccurrences(DateTime.Now, end);
        }
    }
}
