using System;
using System.Runtime.Serialization;
using Itera.MachineLearning.Fitness.Services;

namespace Itera.MachineLearning.Fitness
{
    [DataContract]
    public class WeatherData : ICalendarData
    {
        private WeatherData(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; private set; }

        [DataMember(Name = "TAM")]
        public double Temperature { get; private set; }

        [DataMember(Name = "RR")]
        public double Percipitation { get; private set; }

        [DataMember(Name = "FFM")]
        public double Wind { get; private set; }

        [DataMember(Name = "NNM")]
        public double Cloudness { get; private set; }

        [DataMember(Name = "EE")]
        public StateOfGround StateOfGround { get; private set; }
    }
}