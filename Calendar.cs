using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itera.MachineLearning.Fitness
{
    public class Calendar<TItem>
        where TItem : class
    {
        private readonly Func<TItem, DateTime> dateFunc;
        private readonly Func<TItem, string> contentFunc;

        public const int DaysInAWeek = 7;
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Weeks { get; private set; }
        public IList<TItem> Items { get; private set; }

        public Calendar(
            IList<TItem> items,
            Func<TItem, DateTime> dateFunc,
            Func<TItem, string> contentFunc)
        {
            this.dateFunc = dateFunc;
            this.contentFunc = contentFunc;
            this.Start = items.Min(dateFunc);
            this.End = items.Max(dateFunc);
            this.Duration = End - Start;
            this.Items = items;
            this.Weeks = (int) Math.Ceiling(
                (double) Duration.Days / DaysInAWeek);
        }
        public DateTime GetDate(TItem item)
        {
            return dateFunc(item);
        }
        public string GetContent(TItem item)
        {
            return item == null ? string.Empty : contentFunc(item);
        }

        public string ToHtmlTable()
        {
            var days = Enumerable.Range(1, DaysInAWeek)
                .Select(i => (DayOfWeek)(i % DaysInAWeek))
                .ToArray();

            var html = new StringBuilder("<table><thead>");

            html.Append(string.Join(string.Empty,
                days
                .Select(d => "<th>" + d.ToString() + "</th>")
                .ToArray()));
            html.Append("</thead><tbody>");

            html.Append(string.Join(string.Empty,
                Items
                .GroupBy(d => (GetDate(d).DayOfYear+1) / DaysInAWeek)
                .OrderBy(g => g.Key)
                .Select(g =>
                    "<tr>" +
                    string.Join(string.Empty,
                        days.Select(d =>
                            "<td>" +
                            GetContent(g.SingleOrDefault(i => GetDate(i).DayOfWeek == d)) +
                            "</td>")
                        .ToArray()) +
                    "</tr>")
                .ToArray()));

            html.Append("</tbody></table>");

            return html.ToString();
        }

    }

}
