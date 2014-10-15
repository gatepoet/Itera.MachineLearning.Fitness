using System;

namespace Itera.MachineLearning.Fitness.Services
{
    public class ForecastWeatherData : ICalendarData
    {
        public DateTime Date { get; internal set; }
        public double Temperature { get; internal set; }

        public double Percipitation { get; internal set; }

        public double Wind { get; internal set; }
    }
}