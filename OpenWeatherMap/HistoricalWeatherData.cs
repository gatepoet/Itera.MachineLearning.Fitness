using System;
using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class HistoricalWeatherData : IWeatherData
    {
        [JsonProperty("temp")]
        public DataPoint Temp { get; set; }

        [JsonProperty("pressure")]
        public DataPoint Pressure { get; set; }

        [JsonProperty("humidity")]
        public DataPoint Humidity { get; set; }

        [JsonProperty("calc")]
        public Calc Calc { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("main")]
        public Main Main { get; set; }

        [JsonProperty("dt")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; }
    }

    public interface IWeatherData
    {
        //DateTime Date { get; }
        //double Temperature { get; }
        //double Temperature { get; }
    }
}