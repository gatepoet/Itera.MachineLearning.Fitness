using Newtonsoft.Json;

namespace Itera.MachineLearning.Fitness.OpenWeatherMap
{
    public class Wind
    {
        [JsonProperty("speed")]
        public DataPoint Speed { get; set; }

        [JsonProperty("deg.v")]
        public int Deg { get; set; }

        [JsonProperty("gust")]
        public DataPoint Gust { get; set; }
    }
}