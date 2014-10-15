using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Itera.MachineLearning.Fitness.Services.WeatherHistory
{
    public static class WeatherHistoryExtensions
    {
        public class MeasureProperty
        {
            public PropertyInfo Property { get; internal set; }
            public DataMemberAttribute MeasureKey { get; internal set; }
        }
        public static string[] GetMeasureKeys(this Type type)
        {
            return type.GetMeasureProperties()
                .Select(p => p.MeasureKey.Name)
                .ToArray();
        }

        public static IEnumerable<MeasureProperty> GetMeasureProperties(this Type type)
        {
            return type.GetProperties()
                .Where(p =>
                    p.CanWrite)
                .Select(p => new MeasureProperty
                {
                    Property = p,
                    MeasureKey = p
                        .GetCustomAttributes(
                            typeof(DataMemberAttribute))
                        .SingleOrDefault()
                        as DataMemberAttribute
                })
                .Where(p =>
                    p.MeasureKey != null);
        }
    }
}