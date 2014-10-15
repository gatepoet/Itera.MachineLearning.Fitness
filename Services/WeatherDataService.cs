using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Itera.MachineLearning.Fitness.no.met.eklima;
using Itera.MachineLearning.Fitness.Services.WeatherForecast;
using Itera.MachineLearning.Fitness.Services.WeatherHistory;

namespace Itera.MachineLearning.Fitness.Services
{
    public class WeatherDataService
    {
        private readonly MetDataService historyService;
        private readonly ForecastService forecastService;

        public WeatherDataService()
        {
            historyService = new MetDataService();
            forecastService = new ForecastService();
        }

        public WeatherHistory<T> LastMonth<T>(
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

        public WeatherHistory<T> GetHistoricalMetDataDaily<T>(
            int station,
            DateTime from,
            DateTime to)
            where T :
                class,
                ICalendarData
        {
            var measureKeys = typeof (T).GetMeasureKeys();

            return new WeatherHistory<T>(
                GetHistoricalMetDataDaily(
                    station,
                    measureKeys,
                    from,
                    to));
        }

        private SimpleWeatherHistory GetHistoricalMetDataDaily(int station, string[] measures, DateTime fromDate,
            DateTime toDate)
        {
            var metData = historyService
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

            return new SimpleWeatherHistory(metData);
        }

        public WeatherForecast<ForecastWeatherData> NextWeek(string place)
        {
            var task = Task.Run(() => forecastService.GetForecast(place));
            task.Wait();

            return new WeatherForecast<ForecastWeatherData>(task.Result);
        }
    }
}