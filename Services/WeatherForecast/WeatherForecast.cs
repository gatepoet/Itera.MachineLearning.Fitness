using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Itera.MachineLearning.Fitness.Services
{
    public class WeatherForecast<T> : IWeatherDataCollection<T>
        where T : ICalendarData
    {
        public WeatherForecast(IEnumerable<T> weatherData)
        {
            WeatherData = weatherData.OrderBy(d => d.Date);
            Start = WeatherData.Min(d => d.Date);
            End = WeatherData.Max(d => d.Date);
        }

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public IEnumerable<T> WeatherData { get; private set; }
        
        public IEnumerator<T> GetEnumerator()
        {
            return WeatherData.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}