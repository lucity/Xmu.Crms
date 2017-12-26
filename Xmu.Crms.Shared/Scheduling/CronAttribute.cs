using System;

namespace Xmu.Crms.Shared.Scheduling
{
    [AttributeUsage(AttributeTargets.Method,
                       AllowMultiple = true)]
    public class CronAttribute : Attribute
    {
        public CronAttribute(string schedule)
        {
            Schedule = schedule;
        }

        public string Schedule { get; }
    }
}
