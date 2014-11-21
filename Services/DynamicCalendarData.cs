using System;

namespace Itera.MachineLearning.Fitness.Services
{
    public class DynamicCalendarData : ICalendarData
    {
        private readonly object data;
        public DynamicCalendarData(object data)
        {
            this.data = data;
        }
        public DateTime Date { get { return Get<DateTime>("Date"); } }
        public T Get<T>(string name)
        {
            return (T) data.GetType().GetProperty(name).GetGetMethod().Invoke(data, null);
        }
    }
}