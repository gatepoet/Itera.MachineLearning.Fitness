using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class Main
    {
        [JsonProperty("humidity")]
        public DataPoint Humidity { get; set; }

        [JsonProperty("temp")]
        public DataPoint Temp { get; set; }

        [JsonProperty("temp_max")]
        public double TempMax { get; set; }

        [JsonProperty("pressure")]
        public DataPoint Pressure { get; set; }
    }
}