using System;
using System.Collections.Generic;
using System.Linq;
using Itera.MachineLearning.Fitness.Services;

namespace Itera.MachineLearning.Fitness
{
    public class Calendar<TItem>
        where TItem : class, ICalendarData
    {
        public const int DaysInAWeek = 7;
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Weeks { get; private set; }
        public IList<TItem> Items { get; private set; }

        public Calendar(
            IList<TItem> items,
            Func<TItem, DateTime> dateFunc)
        {
            Start = items.Min(dateFunc);
            End = items.Max(dateFunc);
            Duration = End - Start;
            Items = items;
            Weeks = (int) Math.Ceiling(
                (double) Duration.Days / DaysInAWeek);
        }


    }

}
