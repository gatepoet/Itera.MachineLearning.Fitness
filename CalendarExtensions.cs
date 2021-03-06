using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Itera.MachineLearning.Fitness.Services;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;

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

        public static string ToHtmlContent(this WeatherData forecastWeatherData)
        {
            return string.Format(
                "<span class='date'>{0:d. MMM}</span><br />" +
                "<span class='temperature'>{1}\u00A0<sub>\u2103</sub></span><br />" +
                "<span class='percipitation'>{2}\u00A0<sub>mm</sub></span><br />" +
                "<span class='wind'>{3}\u00A0<sup>m</sup>\u2044<sub>s</sub></span><br />",
                forecastWeatherData.Date,
                forecastWeatherData.Temperature,
                forecastWeatherData.Percipitation,
                forecastWeatherData.Wind);
        }

        public static string ToTypedHtmlContent(this DailyExercisePlan dailyPlan)
        {
            var table = string.Format(
                "<span class='date'>{0:d. MMM}</span><br />" +
                "<span class='temperature'>{1}\u00A0<sub>\u2103</sub></span><br />" +
                "<span class='percipitation'>{2}\u00A0<sub>mm</sub></span><br />" +
                "<span class='wind'>{3}\u00A0<sup>m</sup>\u2044<sub>s</sub></span><br />" +
                "<span class=''>{4}</span><br />" +
                "<span class=''>{5}</span><br />",
                dailyPlan.Date,
                dailyPlan.Temperature,
                dailyPlan.Percipitation,
                dailyPlan.Wind,
                dailyPlan.TypeByWeather,
                dailyPlan.Duration);

            return table;
        }
        public static string ToDetailedHtmlContent(this DailyExercisePlan dailyPlan)
        {
            var table = string.Format(
                "<span class='date'>{0:d. MMM}</span><br />" +
                "<span class='temperature'>{1}\u00A0<sub>\u2103</sub></span><br />" +
                "<span class='percipitation'>{2}\u00A0<sub>mm</sub></span><br />" +
                "<span class='wind'>{3}\u00A0<sup>m</sup>\u2044<sub>s</sub></span><br />" +
                "<span class=''>{4}</span><br />",
                dailyPlan.Date,
                dailyPlan.Temperature,
                dailyPlan.Percipitation,
                dailyPlan.Wind,
                dailyPlan.Duration);

            foreach (var hourlyExercisePlan in dailyPlan.TypeByWeekdayAndHour)
            {
                table += string.Format(
                    "<br />" +
                    "<span><b>{0}</b></span><br />" +
                    "<span>{1:0.00}\u00A0km</span><br />" +
                    "<span>{2}</span><br />" +
                    "<span>{3}</span><br />",
                    hourlyExercisePlan.Activity,
                    hourlyExercisePlan.Distance,
                    hourlyExercisePlan.Speed,
                    string.Join(", ", hourlyExercisePlan.Hours.Select(h => h.ToString() + ":00")));
            }

            return table;
        }

        public static string ToHtmlContent(this HistoricalWeatherData historicalWeatherData)
        {
            return string.Format(
                "<span class='date'>{0:d. MMM}</span><br />" +
                "<span class='temperature'>{1}\u00A0<sub>\u2103</sub></span><br />" +
                "<span class='percipitation'>{2}\u00A0<sub>mm</sub></span><br />" +
                "<span class='wind'>{3}\u00A0<sup>m</sup>\u2044<sub>s</sub></span><br />" +
                "<span class='cloudness'>{4}</span><br />" +
                "<span class='stateOfGround'>{5}</span><br />",
                historicalWeatherData.Date,
                historicalWeatherData.Temperature,
                historicalWeatherData.Percipitation,
                historicalWeatherData.Wind,
                historicalWeatherData.Cloudness,
                historicalWeatherData.StateOfGround);
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
            var cal = CultureInfo.CreateSpecificCulture("no").Calendar;
            
            html.Append(string.Join(string.Empty,
                calendar.Items
                    .GroupBy(i => cal.GetWeekOfYear(i.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
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