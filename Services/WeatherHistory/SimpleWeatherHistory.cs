using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Itera.MachineLearning.Fitness.no.met.eklima;

namespace Itera.MachineLearning.Fitness.Services.WeatherHistory
{
    internal class SimpleWeatherHistory : IWeatherDataCollection<SimpleWeatherData>
    {
        private readonly IList<SimpleWeatherData> weatherData;
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        internal SimpleWeatherHistory(no_met_metdata_Metdata metData)
        {
            weatherData = metData.timeStamp
                .Select(ts => new SimpleWeatherData(
                    ts.from,
                    ts.location
                        .Single().weatherElement
                        .ToDictionary(we => we.id, we => we.value)))
                .OrderBy(wd => wd.Date)
                .ToList();

            Start = weatherData.Min(wd => wd.Date);
            End = weatherData.Max(wd => wd.Date);
        }

        public IEnumerator<SimpleWeatherData> GetEnumerator()
        {
            return weatherData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}