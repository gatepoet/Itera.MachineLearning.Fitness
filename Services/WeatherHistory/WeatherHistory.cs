using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.Services.WeatherHistory
{
    public class WeatherHistory<T> :
        IWeatherDataCollection<T>
        where T : class, ICalendarData
    {
        private static readonly ConstructorInfo Constructor =
            typeof(T).GetConstructor(
                BindingFlags.NonPublic |
                BindingFlags.Instance,
                null,
                new[] { typeof(DateTime) },
                null);
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public IEnumerable<T> WeatherData { get; private set; }

        internal WeatherHistory(
            SimpleWeatherHistory weatherData)
        {
            Contract.Assert(Constructor != null,
                string.Format(
                    "Type '{0}' needs a constructor which takes a DateTime parameter.",
                    typeof(T).FullName));

            Start = weatherData.Start;
            End = weatherData.End;
            WeatherData = weatherData.Select(
                MapObject);
        }

        private static T MapObject(SimpleWeatherData d)
        {
            var obj = Constructor
                .Invoke(new object[] {
                    d.Date})
                as T;

            typeof(T).GetMeasureProperties()
                .ToList()
                .ForEach(p => p.Property
                    .SetValue(
                        obj,
                        JsonConvert.DeserializeObject(
                            d.Measures[p.MeasureKey.Name],
                            p.Property.PropertyType)
                    ));

            return obj;
        }


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