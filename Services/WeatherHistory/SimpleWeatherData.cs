using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Itera.MachineLearning.Fitness.Services.WeatherHistory
{
    public class SimpleWeatherData : ICalendarData
    {
        public DateTime Date { get; private set; }
        public ReadOnlyDictionary<string, string> Measures { get; private set; }

        private SimpleWeatherData(DateTime date)
        {
            Date = date;           
        }
        public SimpleWeatherData(
            DateTime date,
            Dictionary<string, string> measures)
            :
            this(date)
        {
            Measures = new ReadOnlyDictionary<string, string>(measures);
        }
    }
}