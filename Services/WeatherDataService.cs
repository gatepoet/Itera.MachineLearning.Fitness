using System;
using System.Globalization;
using Itera.MachineLearning.Fitness.no.met.eklima;

namespace Itera.MachineLearning.Fitness.Services
{
    public class WeatherDataService
    {
        private readonly MetDataService service;

        public WeatherDataService()
        {
            service = new MetDataService();
        }

        public WeatherDataCollection<T> LastMonth<T>(
            int station)
            where T :
                class,
                ICalendarData
        {
            var to = DateTime.Today.Subtract(
                TimeSpan.FromDays(1));
            var from = to.Subtract(
                TimeSpan.FromDays(30));

           return GetHistoricalMetDataDaily<T>(station, from, to);
        }

        public WeatherDataCollection<T> GetHistoricalMetDataDaily<T>(
            int station,
            DateTime from,
            DateTime to)
            where T :
                class,
                ICalendarData
        {
            var measureKeys = typeof (T).GetMeasureKeys();

            return new WeatherDataCollection<T>(
                GetHistoricalMetDataDaily(
                    station,
                    measureKeys,
                    from,
                    to));
        }

        private SimpleWeatherDataCollection GetHistoricalMetDataDaily(int station, string[] measures, DateTime fromDate,
            DateTime toDate)
        {
            var metData = service
                .getMetData(
                    "0",
                    "",
                    fromDate.ToString("yyyy-MM-dd"),
                    toDate.ToString("yyyy-MM-dd"),
                    station.ToString(CultureInfo.InvariantCulture),
                    string.Join(",", measures),
                    string.Empty,
                    string.Empty,
                    string.Empty);

            return new SimpleWeatherDataCollection(metData);
        }
    }
}