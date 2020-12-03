using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SeleniumScript.Utilities
{
    internal static class RetryWaitCalculator
    {
        public static readonly TimeSpan DefaultWait = TimeSpan.MinValue;

        public static TimeSpan GetTimeSpanForWait(int retryNumber, ICollection<TimeSpan> timespans)
        {
            if(timespans == null || timespans.Count == 0)
            {
                return DefaultWait;
            }

            return timespans.ElementAt((retryNumber - 1) % timespans.Count);
        }
    }
}
