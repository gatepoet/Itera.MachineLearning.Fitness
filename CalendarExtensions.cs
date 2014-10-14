using System;
using System.Linq;
using System.Text;
using Itera.MachineLearning.Fitness.Services;

namespace Itera.MachineLearning.Fitness
{
    public static class CalendarExtensions
    {
        public static string GetContent<TItem>(
            Func<TItem, string> contentFunc,
            TItem item)
            where TItem : class
        {
            return item == null
                ? string.Empty
                : contentFunc(item);
        }

        public static string ToHtmlContent(this WeatherData weatherData)
        {
            return string.Format(
                "<span class='date'>{0:d. MMM}</span><br />" +
                "<span class='temperature'>{1}\u00A0<sub>\u2103</sub></span><br />" +
                "<span class='percipitation'>{2}\u00A0<sub>mm</sub></span><br />" +
                "<span class='wind'>{3}\u00A0<sup>m</sup>\u2044<sub>s</sub></span><br />" +
                "<span class='cloudness'>{4}</span><br />" +
                "<span class='stateOfGround'>{5}</span><br />",
                weatherData.Date,
                weatherData.Temperature,
                weatherData.Percipitation,
                weatherData.Wind,
                weatherData.Cloudness,
                weatherData.StateOfGround);
        }

        public static string ToHtmlTable<TItem>(this
            Calendar<TItem> calendar,
            Func<TItem, string> contentFunc)
            where TItem : class, ICalendarData
        {
            var days = Enumerable.Range(1, Calendar<TItem>.DaysInAWeek)
                .Select(i => (DayOfWeek)(i % Calendar<TItem>.DaysInAWeek))
                .ToArray();

            var html = new StringBuilder(
                "<table class='calendar'>\n" +
                "\t<thead>\n" +
                "\t\t<tr>\n");

            html.Append(string.Join(string.Empty,
                days
                    .Select(day => "\t\t\t<th>" + day.ToString() + "</th>\n")
                    .ToArray()));

            html.Append("\t\t</tr>\n" +
                        "\t</thead>\n" +
                        "\t<tbody>\n");

            html.Append(string.Join(string.Empty,
                calendar.Items
                    .GroupBy(i => (i.Date.DayOfYear + 1) / Calendar<TItem>.DaysInAWeek)
                    .OrderBy(g => g.Key)
                    .Select(g =>
                        "\t\t<tr>\n" +
                        string.Join(string.Empty,
                            days.Select(day =>
                            {
                                var content = GetContent(contentFunc,
                                    g.SingleOrDefault(i =>
                                        i.Date.DayOfWeek == day));
                                var noDataClass = content == string.Empty
                                    ? string.Format(" no-data")
                                    : string.Empty;

                                return "\t\t\t<td class='" + day.ToString().ToLower() + noDataClass + "'>" + content + "</td>\n";
                            })
                                .ToArray()) +
                        "\t\t</tr>\n")
                    .ToArray()));

            html.Append("\t</tbody>\n" +
                        "</table>\n");

            return html.ToString();
        }        
    }
}