using System;
using System.Collections.Generic;
using System.Threading;
using CC_Library.Predictions;

namespace Trader
{
    internal class TaskScheduler
    {
        private static TaskScheduler _instance;
        private List<Timer> timers = new List<Timer>();

        private TaskScheduler() { }

        public static TaskScheduler Instance => _instance ?? (_instance = new TaskScheduler());

        public void ScheduleCycle(double intervalInHour)
        {
            List<StonkValues> vals = new List<StonkValues>();

            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, 0);
            while (now > firstRun)
            {
                firstRun = firstRun.AddHours(intervalInHour);
            }

            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }

            var timer = new Timer(x =>
            {
                _ = BuySell.GetMarketData(vals);
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));

            timers.Add(timer);
        }
    }
}
