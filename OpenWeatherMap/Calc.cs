using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class Calc
    {
        [JsonProperty("dewpoint")]
        public DataPoint DewPoint { get; set; }

        [JsonProperty("humidex")]
        public DataPoint Humidex { get; set; }

        [JsonProperty("headindex")]
        public DataPoint HeatIndex { get; set; }
    }
}